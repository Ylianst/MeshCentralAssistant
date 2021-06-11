using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using static MeshAssistant.Win32Api;

namespace MeshAssistant
{
    public class ConPTY
    {
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
        static public class ProcessFactory
        {
            /// <summary>
            /// Start and configure a process. The return value represents the process and should be disposed.
            /// </summary>
            internal static XProcess Start(string command, IntPtr attributes, IntPtr hPC)
            {
                var startupInfo = ConfigureProcessThread(hPC, attributes);
                var processInfo = RunProcess(ref startupInfo, command);
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

            public void Resize(int width, int height)
            {
                ResizePseudoConsole(this.Handle, new COORD { X = (short)width, Y = (short)height });
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
                try { ClosePseudoConsole(Handle); } catch (Exception) { }
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
