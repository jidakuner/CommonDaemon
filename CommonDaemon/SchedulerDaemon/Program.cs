using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerDaemon
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                foreach (string key in ConfigurationManager.AppSettings.AllKeys)
                {
                    var process = ProcessWrapper.CreateWatcher(key, ConfigurationManager.AppSettings[key]);

                    if (!process.IsActive())
                    {
                        process.Start();
                    }

                    Logger.AppendLogForLog("Servie is OK for \"{0}\"", key);
                }
            }
            catch (Exception e)
            {
                Logger.AppendLogForError("Error in Initalize service, message: \"{0}\"", e.Message);
            }
        }
    }
}
