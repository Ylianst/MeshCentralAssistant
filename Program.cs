/*
Copyright 2009-2020 Intel Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MeshAssistant
{
    static class Program
    {
        [ThreadStatic]
        public static readonly bool IsMainThread = true;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string update = null;
            foreach (string arg in args)
            {
                if (arg.Length > 8 && arg.Substring(0, 8).ToLower() == "-update:") { update = arg.Substring(8); }
            }

            if (update != null)
            {
                // Perform self update, no mutex
                MainForm main = new MainForm(args);
            }
            else
            {
                // Named Mutexes are available computer-wide. Use a unique name.
                using (var mutex = new Mutex(false, "MeshCentralAssistantSingletonMutex"))
                {
                    // TimeSpan.Zero to test the mutex's signal state and
                    // return immediately without blocking
                    bool isAnotherInstanceOpen = !mutex.WaitOne(TimeSpan.Zero);
                    if (isAnotherInstanceOpen) { return; }

                    // Setup settings & visual style
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Properties.Settings.Default.Upgrade();

                    Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ExceptionSink);
                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionEventSink);
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException, true);

                    foreach (string arg in args)
                    {
                        if (arg.Length > 3 && string.Compare(arg.Substring(0, 3), "-l:", true) == 0)
                        {
                            try { Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(arg.Substring(3)); } catch (ArgumentException) { }
                        }
                    }

                    MainForm main;
                    System.Globalization.CultureInfo currentCulture;
                    do
                    {
                        currentCulture = Thread.CurrentThread.CurrentUICulture;
                        main = new MainForm(args);
                        if (main.forceExit == false) { Application.Run(main); }
                    }
                    while (currentCulture.Equals(Thread.CurrentThread.CurrentUICulture) == false);

                    mutex.ReleaseMutex();
                }
            }
        }

        public static void Debug(string msg) { try { File.AppendAllText("debug.log", msg + "\r\n"); } catch (Exception) { } }

        public static void ExceptionSink(object sender, System.Threading.ThreadExceptionEventArgs args)
        {
            Debug("ExceptionSink: " + args.Exception.ToString());
        }

        public static void UnhandledExceptionEventSink(object sender, UnhandledExceptionEventArgs args)
        {
            Debug("UnhandledExceptionEventSink: " + ((Exception)args.ExceptionObject).ToString());
        }
    }
}
