using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Configuration;
using System.Threading;

namespace CommonDaemon
{
    public partial class CommonDaemon : ServiceBase
    {
        private Dictionary<string, string> MaintainList = new Dictionary<string, string>();

        private List<Watcher> Watchers = new List<Watcher>();

        private Thread MainThread;

        public CommonDaemon()
        {
            InitializeComponent();

            this.ServiceName = "CommonDaemonService";
            this.EventLog.Log = "Application";

            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;

            try
            {
                foreach(string key in ConfigurationManager.AppSettings.AllKeys)
                {
                    if (!this.MaintainList.ContainsKey(key))
                    {
                        this.MaintainList.Add(key, ConfigurationManager.AppSettings[key]);

                        Logger.AppendLogForLog("Get watched process: \"{0}\" with parmeters \"{1}\"", key, ConfigurationManager.AppSettings[key]);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.AppendLogForError("Error in Initalize service, message: \"{0}\"", e.Message);
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Logger.AppendLogForLog("Begin OnStart service");
                foreach (var pair in this.MaintainList)
                {
                    Watcher watcher = Watcher.CreateWatcher(pair.Key, pair.Value);

                    if (watcher != null)
                    {
                        watcher.Start();
                        this.Watchers.Add(watcher);
                    }
                    else
                    {
                        Logger.AppendLogForError("Error in OnStart service for \"{0}\"", pair.Key);
                    }
                }

                this.MainThread = new Thread(new ThreadStart(ServiceMain));
                this.MainThread.Start();

                Logger.AppendLogForLog("End OnStart service");
            }
            catch (Exception e)
            {
                Logger.AppendLogForError("Error in OnStart service, message: \"{0}\"", e.Message);
            }
        }

        protected override void OnStop()
        {
            try
            {
                Logger.AppendLogForLog("Begin OnStop service");
                foreach (var watcher in this.Watchers)
                {
                    watcher.Close();
                }

                MainThread.Abort();

                Logger.AppendLogForLog("End OnStop service");
            }
            catch (Exception e)
            {
                Logger.AppendLogForError("Error in OnStop service, message: \"{0}\"", e.Message);
            }
        }

        private void ServiceMain()
        {
            if (this.Watchers == null)
            {
                return;
            }

            while (true)
            {
                foreach (var worker in this.Watchers)
                {
                    if (worker.Process == null)
                    {
                        worker.Start();
                        Logger.AppendLogForLog("Start watcher for \"{0}\"", worker.FileName);
                    }
                    else if (worker.Process.HasExited)
                    {
                        worker.Restart();
                        Logger.AppendLogForLog("Restart watcher for \"{0}\"", worker.FileName);
                    }
                    else
                    {
                        Logger.AppendLogForLog("All thing is good for \"{0}\"", worker.FileName);
                    }
                }

                Thread.Sleep(new TimeSpan(0, 1, 0));
            }
        }
    }
}
