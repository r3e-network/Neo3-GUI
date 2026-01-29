using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Neo.Common
{
    public class CommandLineTool
    {
        private const string WindowsShell = "cmd";
        private const string UnixShell = "bash";
        private const string WindowsShellArg = "/c ";
        private const string UnixShellArg = "-c ";
        private const string CloseMessage = "close";

        public static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static readonly bool IsMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static readonly bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static readonly ConcurrentQueue<Process> CurrentProcesses = new ConcurrentQueue<Process>();

        private static string shell => IsWindows ? WindowsShell : UnixShell;

        private static string shellArg => IsWindows ? WindowsShellArg : UnixShellArg;
        

        public static Process Run(string command, string workDirectory = "")
        {
            Process p = new Process();
            p.StartInfo.FileName = shell;
            p.StartInfo.Arguments = shellArg + command;
            p.StartInfo.WorkingDirectory = workDirectory;
            p.OutputDataReceived += (s, r) =>
            {
                if (r.Data == null)
                {
                    Console.WriteLine(CloseMessage);
                    p = null;
                    return;
                }
                Console.WriteLine(r.Data);
            };
            p.Start();
            CurrentProcesses.Enqueue(p);
            return p;
        }

        public static Process Run(string command, string workDirectory = "", Action<string> receiveOutput = null)
        {
            Process p = new Process();
            p.StartInfo.FileName = shell;
            p.StartInfo.WorkingDirectory = workDirectory;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += (s, r) =>
            {
                if (r.Data == null)
                {
                    Console.WriteLine(CloseMessage);
                    p = null;
                    return;
                }
            };
            p.Start();
            p.StandardInput.WriteLine(command);
            p.BeginOutputReadLine();
            CurrentProcesses.Enqueue(p);
            return p;
        }


        public static void Close()
        {
            foreach (var currentProcess in CurrentProcesses)
            {
                currentProcess.Kill();
            }
        }
    }
}
