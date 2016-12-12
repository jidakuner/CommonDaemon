using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerDaemon
{
    public class ProcessWrapper
    {
        public string Tag = string.Empty;
        public string FileName = string.Empty;
        public string Arguments = string.Empty;

        public Process Process = null;

        private ProcessWrapper(string tag, string fileName, string arguments)
        {
            this.Tag = tag;
            this.FileName = fileName;
            this.Arguments = arguments;
        }

        static public string ParseFileName(string args)
        {
            int a = args.IndexOf(" /");
            int b = args.IndexOf(@" \");

            int pointDir = a < 0 ? (b < 0 ? args.Length : b) : (b < 0 ? a : Math.Max(a, b));

            string dir = Path.GetDirectoryName(args.Substring(0, pointDir));

            int c = args.IndexOf(" ", dir.Length);
            int pointFile = c < 0 ? args.Length : c;

            return args.Substring(0, pointFile);
        }

        static public ProcessWrapper CreateWatcher(string tag, string args)
        {
            string fileName = ParseFileName(args);
            string arguments = args.Substring(fileName.Length);

            if (File.Exists(fileName))
            {
                ProcessWrapper watcher = new ProcessWrapper(tag, fileName, arguments);

                return watcher;
            }
            else
            {
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
                    this.Process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    this.Process.Start();
                    
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
            if (this.Process != null && !this.Process.HasExited)
            {
                try
                {
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

        public bool IsActive( string url)
        {            
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch(Exception e)
            {
                Logger.AppendLogForError("Cannot get IsActive of process \"{0}\", because \"{1}\"", this.FileName, e.Message);
                return false;
            }
        }
    }
}
