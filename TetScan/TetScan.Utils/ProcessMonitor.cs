using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetScan.TetScan.Utils
{
    public class ProcessMonitor : IDisposable
    {
        List<Process> processes = new List<Process>();

        public Process Start(ProcessStartInfo info)
        {
            var newProcess = Process.Start(info);
            newProcess.EnableRaisingEvents = true;
            processes.Add(newProcess);
            newProcess.Exited += (sender, e) => processes.Remove(newProcess);
            return newProcess;
        }

        ~ProcessMonitor()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            foreach (var process in processes)
            {
                try
                {
                    if (!process.HasExited)
                        process.Kill();
                }
                catch { }
            }
        }

        public static void KillProcessAndChildren(int pid)
        {
            using (var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid))
            using (ManagementObjectCollection moc = searcher.Get())
            {
                foreach (ManagementObject mo in moc)
                {
                    KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
                }
                try
                {
                    Process proc = Process.GetProcessById(pid);
                    proc.Kill();
                }
                catch (ArgumentException)
                { }
            }
        }
    }
}
