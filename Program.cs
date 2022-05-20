/*
Copyright 2009-2022 Intel Corporation

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
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace MeshAssistant
{
    static class Program
    {
        [ThreadStatic]
        public static readonly bool IsMainThread = true;
        public static string LockToHostname = null;
        public static string LockToServerId = null;

        public class CurrentAppContext : ApplicationContext
        {
            private static CurrentAppContext _currContext;

            public CurrentAppContext() { if (_currContext == null) { _currContext = this; } }

            public CurrentAppContext(Form AppMainForm) : this() { this.MainForm = AppMainForm; }

            public CurrentAppContext CurrentContext { get { return _currContext; } }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // If this application is signed, get the URL of the signature, this will be used to lock this application to a server.
            Uri signedUrl = WinCrypt.GetSignatureUrl(System.Reflection.Assembly.GetEntryAssembly().Location);
            if (signedUrl != null)
            {
                NameValueCollection urlArguments = HttpUtility.ParseQueryString(signedUrl.Query);
                if (urlArguments["serverid"] != null)
                {
                    LockToServerId = urlArguments["serverid"];
                    LockToHostname = signedUrl.Host;
                }
            }

            if (Environment.OSVersion.Version.Major >= 6) SetProcessDPIAware();

            string update = null;
            foreach (string arg in args)
            {
                if ((arg.Length > 3) && (string.Compare(arg.Substring(0, 3), "-l:", true) == 0))
                {
                    try { Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(arg.Substring(3)); } catch (ArgumentException) { }
                }
                if ((arg.Length == 5) && (string.Compare(arg.Substring(0, 5), "-info", true) == 0))
                {
                    string dialogText = string.Format(Properties.Resources.Version, System.Reflection.Assembly.GetExecutingAssembly().ImageRuntimeVersion);
                    if (LockToHostname != null) { dialogText += "\r\n" + string.Format(Properties.Resources.LockedToHost, LockToHostname); }
                    if (LockToServerId != null) { dialogText += "\r\n" + string.Format(Properties.Resources.LockedToServerId, LockToServerId); }
                    MessageBox.Show(dialogText, Properties.Resources.MeshCentralAssistant);
                    return;
                }
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

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }

}
