using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Threading;

namespace CommonDaemon
{
    public class Watcher
    {
        public string FileName = string.Empty;
        public string Arguments = string.Empty;

        public Process Process = null;

        private Watcher(string fileName, string arguments)
        {
            this.FileName = fileName;
            this.Arguments = arguments;
        }

        static public Watcher CreateWatcher(string fileName, string arguments)
        {
            try
            {

                if (File.Exists(fileName))
                {
                    Watcher watcher = new Watcher(fileName, arguments);
                    
                    return watcher;
                }
                else
                {
                    Logger.AppendLogForError("Cannot Watch process \"{0}\", because cannot find the file", fileName);
                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.AppendLogForError("Cannot Watch process \"{0}\", because \"{1}\"", fileName, e.Message);

                return null;
            }
        }
        
        public void Start()
        {
            if (this.Process == null)
            {
                try
                {
                    this.Process = new Process();
                    this.Process.StartInfo.FileName = this.FileName;
                    this.Process.StartInfo.Arguments = this.Arguments;
                    this.Process.StartInfo.CreateNoWindow = true;
                    this.Process.Start();

                    Logger.AppendLogForLog("Start process \"{0}\"", this.FileName);
                    return;
                }
                catch (Exception e)
                {
                    Logger.AppendLogForError("Cannot start process \"{0}\", because \"{1}\"", this.FileName, e.Message);
                }
            }
            else
            {
                this.Restart();
            }
            
        }

        public void Restart()
        {
            if (this.Process != null)
            {
                try
                {
                    this.Close();
                    this.Start();

                    Logger.AppendLogForLog("Restart process \"{0}\"", this.FileName);
                    return;
                }
                catch (Exception e)
                {
                    Logger.AppendLogForError("Cannot restart process \"{0}\", because \"{1}\"", this.FileName, e.Message);
                }
            }
            else
            {
                this.Start();
            }

        }

        public void Close()
        {
            if (this.Process != null)
            {
                try
                {
                    //CloseWithChildren(this.Process.Id);
                    this.Process.Close();

                    Logger.AppendLogForLog("Close process \"{0}\"", this.FileName);
                    return;
                }
                catch (Exception e)
                {
                    Logger.AppendLogForError("Cannot close process \"{0}\", because \"{1}\"", this.FileName, e.Message);
                }
            }
        }

        private void CloseWithChildren(int pid)
        {
            using (var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid))
            using (ManagementObjectCollection moc = searcher.Get())
            {
                foreach (ManagementObject mo in moc)
                {
                    CloseById(Convert.ToInt32(mo["ProcessID"]));
                }

                CloseById(pid);
            }
        }

        private void CloseById(int id)
        {
            using (Process p = Process.GetProcessById(id))
            {
                if (p != null && !p.HasExited)
                {
                    p.Kill();
                }
            }
        }
    }


}
