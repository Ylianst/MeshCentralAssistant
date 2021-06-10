using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static MeshAssistant.Win32Api;

namespace MeshAssistant
{
    public class ConPTY
    {

        /// <summary>
        /// The UI of the terminal. It's just a normal console window, but we're managing the input/output.
        /// In a "real" project this could be some other UI.
        /// </summary>
        internal sealed class Terminal
        {
            private const string ExitCommand = "exit\r";
            private const string CtrlC_Command = "\x3";

            public Terminal()
            {
                EnableVirtualTerminalSequenceProcessing();
            }

            /// <summary>
            /// Newer versions of the windows console support interpreting virtual terminal sequences, we just have to opt-in
            /// </summary>
            private static void EnableVirtualTerminalSequenceProcessing()
            {
                uint outConsoleMode;
                var hStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
                if (!GetConsoleMode(hStdOut, out outConsoleMode)) { throw new InvalidOperationException("Could not get console mode"); }
                outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
                if (!SetConsoleMode(hStdOut, outConsoleMode)) { throw new InvalidOperationException("Could not enable virtual terminal processing"); }
            }

            /// <summary>
            /// Start the psuedoconsole and run the process as shown in 
            /// https://docs.microsoft.com/en-us/windows/console/creating-a-pseudoconsole-session#creating-the-pseudoconsole
            /// </summary>
            /// <param name="command">the command to run, e.g. cmd.exe</param>
            public void Run(string command)
            {
                using (var inputPipe = new PseudoConsolePipe())
                using (var outputPipe = new PseudoConsolePipe())
                using (var pseudoConsole = PseudoConsole.Create(inputPipe.ReadSide, outputPipe.WriteSide, (short)Console.WindowWidth, (short)Console.WindowHeight))
                using (var process = ProcessFactory.Start(command, PseudoConsole.PseudoConsoleThreadAttribute, pseudoConsole.Handle))
                {
                    // copy all pseudoconsole output to stdout
                    Task.Run(() => CopyPipeToOutput(outputPipe.ReadSide));
                    // prompt for stdin input and send the result to the pseudoconsole
                    Task.Run(() => CopyInputToPipe(inputPipe.WriteSide));
                    // free resources in case the console is ungracefully closed (e.g. by the 'x' in the window titlebar)
                    OnClose(() => DisposeResources(process, pseudoConsole, outputPipe, inputPipe));

                    WaitForExit(process).WaitOne(Timeout.Infinite);
                }
            }

            /// <summary>
            /// Reads terminal input and copies it to the PseudoConsole
            /// </summary>
            /// <param name="inputWriteSide">the "write" side of the pseudo console input pipe</param>
            private static void CopyInputToPipe(SafeFileHandle inputWriteSide)
            {
                using (var writer = new StreamWriter(new FileStream(inputWriteSide, FileAccess.Write)))
                {
                    ForwardCtrlC(writer);
                    writer.AutoFlush = true;
                    writer.WriteLine(@"cd \");

                    while (true)
                    {
                        // send input character-by-character to the pipe
                        char key = Console.ReadKey(intercept: true).KeyChar;
                        writer.Write(key);
                    }
                }
            }

            /// <summary>
            /// Don't let ctrl-c kill the terminal, it should be sent to the process in the terminal.
            /// </summary>
            private static void ForwardCtrlC(StreamWriter writer)
            {
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    writer.Write(CtrlC_Command);
                };
            }

            /// <summary>
            /// Reads PseudoConsole output and copies it to the terminal's standard out.
            /// </summary>
            /// <param name="outputReadSide">the "read" side of the pseudo console output pipe</param>
            private static void CopyPipeToOutput(SafeFileHandle outputReadSide)
            {
                using (var terminalOutput = Console.OpenStandardOutput())
                using (var pseudoConsoleOutput = new FileStream(outputReadSide, FileAccess.Read))
                {
                    pseudoConsoleOutput.CopyTo(terminalOutput);
                }
            }

            /// <summary>
            /// Get an AutoResetEvent that signals when the process exits
            /// </summary>
            private static AutoResetEvent WaitForExit(XProcess process) =>
                new AutoResetEvent(false)
                {
                    SafeWaitHandle = new SafeWaitHandle(process.ProcessInfo.hProcess, ownsHandle: false)
                };

            /// <summary>
            /// Set a callback for when the terminal is closed (e.g. via the "X" window decoration button).
            /// Intended for resource cleanup logic.
            /// </summary>
            private static void OnClose(Action handler)
            {
                SetConsoleCtrlHandler(eventType =>
                {
                    if (eventType == CtrlTypes.CTRL_CLOSE_EVENT)
                    {
                        handler();
                    }
                    return false;
                }, true);
            }

            private void DisposeResources(params IDisposable[] disposables)
            {
                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }
            }
        }


        /// <summary>
        /// Represents an instance of a process.
        /// </summary>
        internal sealed class XProcess : IDisposable
        {
            public XProcess(STARTUPINFOEX startupInfo, PROCESS_INFORMATION processInfo)
            {
                StartupInfo = startupInfo;
                ProcessInfo = processInfo;
            }

            public STARTUPINFOEX StartupInfo { get; }
            public PROCESS_INFORMATION ProcessInfo { get; }

            #region IDisposable Support

            private bool disposedValue = false; // To detect redundant calls

            void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // dispose managed state (managed objects).
                    }

                    // dispose unmanaged state

                    // Free the attribute list
                    if (StartupInfo.lpAttributeList != IntPtr.Zero)
                    {
                        DeleteProcThreadAttributeList(StartupInfo.lpAttributeList);
                        Marshal.FreeHGlobal(StartupInfo.lpAttributeList);
                    }

                    // Close process and thread handles
                    if (ProcessInfo.hProcess != IntPtr.Zero)
                    {
                        CloseHandle(ProcessInfo.hProcess);
                    }
                    if (ProcessInfo.hThread != IntPtr.Zero)
                    {
                        CloseHandle(ProcessInfo.hThread);
                    }

                    disposedValue = true;
                }
            }

            ~XProcess()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(false);
            }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // use the following line if the finalizer is overridden above.
                GC.SuppressFinalize(this);
            }

            #endregion
        }

        /// <summary>
        /// Support for starting and configuring processes.
        /// </summary>
        /// <remarks>
        /// Possible to replace with managed code? The key is being able to provide the PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE attribute
        /// </remarks>
        static class ProcessFactory
        {
            /// <summary>
            /// Start and configure a process. The return value represents the process and should be disposed.
            /// </summary>
            internal static XProcess Start(string command, IntPtr attributes, IntPtr hPC)
            {
                var startupInfo = ConfigureProcessThread(hPC, attributes);
                var processInfo = RunProcess(ref startupInfo, "cmd.exe");
                return new XProcess(startupInfo, processInfo);
            }

            private static STARTUPINFOEX ConfigureProcessThread(IntPtr hPC, IntPtr attributes)
            {
                // this method implements the behavior described in https://docs.microsoft.com/en-us/windows/console/creating-a-pseudoconsole-session#preparing-for-creation-of-the-child-process

                var lpSize = IntPtr.Zero;
                var success = InitializeProcThreadAttributeList(
                    lpAttributeList: IntPtr.Zero,
                    dwAttributeCount: 1,
                    dwFlags: 0,
                    lpSize: ref lpSize
                );
                if (success || lpSize == IntPtr.Zero) // we're not expecting `success` here, we just want to get the calculated lpSize
                {
                    throw new InvalidOperationException("Could not calculate the number of bytes for the attribute list. " + Marshal.GetLastWin32Error());
                }

                var startupInfo = new STARTUPINFOEX();
                startupInfo.StartupInfo.cb = Marshal.SizeOf<STARTUPINFOEX>();
                startupInfo.lpAttributeList = Marshal.AllocHGlobal(lpSize);

                success = InitializeProcThreadAttributeList(
                    lpAttributeList: startupInfo.lpAttributeList,
                    dwAttributeCount: 1,
                    dwFlags: 0,
                    lpSize: ref lpSize
                );
                if (!success)
                {
                    throw new InvalidOperationException("Could not set up attribute list. " + Marshal.GetLastWin32Error());
                }

                success = UpdateProcThreadAttribute(
                    lpAttributeList: startupInfo.lpAttributeList,
                    dwFlags: 0,
                    Attribute: attributes,
                    lpValue: hPC,
                    cbSize: (IntPtr)IntPtr.Size,
                    lpPreviousValue: IntPtr.Zero,
                    lpReturnSize: IntPtr.Zero
                );
                if (!success)
                {
                    throw new InvalidOperationException("Could not set pseudoconsole thread attribute. " + Marshal.GetLastWin32Error());
                }

                return startupInfo;
            }

            private static PROCESS_INFORMATION RunProcess(ref STARTUPINFOEX sInfoEx, string commandLine)
            {
                int securityAttributeSize = Marshal.SizeOf<SECURITY_ATTRIBUTES>();
                var pSec = new SECURITY_ATTRIBUTES { nLength = securityAttributeSize };
                var tSec = new SECURITY_ATTRIBUTES { nLength = securityAttributeSize };
                PROCESS_INFORMATION pInfo;
                var success = CreateProcess(
                    lpApplicationName: null,
                    lpCommandLine: commandLine,
                    lpProcessAttributes: ref pSec,
                    lpThreadAttributes: ref tSec,
                    bInheritHandles: false,
                    dwCreationFlags: EXTENDED_STARTUPINFO_PRESENT,
                    lpEnvironment: IntPtr.Zero,
                    lpCurrentDirectory: null,
                    lpStartupInfo: ref sInfoEx,
                    lpProcessInformation: out pInfo
                );
                if (!success)
                {
                    throw new InvalidOperationException("Could not create process. " + Marshal.GetLastWin32Error());
                }

                return pInfo;
            }
        }


        /// <summary>
        /// Utility functions around the new Pseudo Console APIs
        /// </summary>
        internal sealed class PseudoConsole : IDisposable
        {
            public static readonly IntPtr PseudoConsoleThreadAttribute = (IntPtr)PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE;

            public IntPtr Handle { get; }

            private PseudoConsole(IntPtr handle)
            {
                this.Handle = handle;
            }

            internal static PseudoConsole Create(SafeFileHandle inputReadSide, SafeFileHandle outputWriteSide, int width, int height)
            {
                IntPtr hPC;
                var createResult = CreatePseudoConsole( new COORD { X = (short)width, Y = (short)height }, inputReadSide, outputWriteSide, 0, out hPC);
                if (createResult != 0) { throw new InvalidOperationException("Could not create psuedo console. Error Code " + createResult); }
                return new PseudoConsole(hPC);
            }

            public void Dispose()
            {
                ClosePseudoConsole(Handle);
            }
        }

        internal sealed class PseudoConsolePipe : IDisposable
        {
            public readonly SafeFileHandle ReadSide;
            public readonly SafeFileHandle WriteSide;

            public PseudoConsolePipe()
            {
                if (!CreatePipe(out ReadSide, out WriteSide, IntPtr.Zero, 0))
                {
                    throw new InvalidOperationException("failed to create pipe");
                }
            }

            #region IDisposable

            void Dispose(bool disposing)
            {
                if (disposing)
                {
                    ReadSide?.Dispose();
                    WriteSide?.Dispose();
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion
        }
        
    }
}
