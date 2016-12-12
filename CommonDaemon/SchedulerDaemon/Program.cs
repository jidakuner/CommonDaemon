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
                foreach (string key in ConfigurationManager.AppSettings["Services"].Split(';'))
                {
                    var process = ProcessWrapper.CreateWatcher(key, ConfigurationManager.AppSettings[key]);

                    string urlKey = string.Format("{0}-URL", key);
                    string url = ConfigurationManager.AppSettings.AllKeys.Contains(urlKey) ? ConfigurationManager.AppSettings[urlKey] : string.Empty;
                    if (!process.IsActive(url))
                    {
                        process.Start();
                        Logger.AppendLogForLog("Servie \"{0}\" have been started.", key);
                    }
                    else
                    {
                        Logger.AppendLogForLog("Servie is running for \"{0}\".", key);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.AppendLogForError("Error in Initalize service, message: \"{0}\".", e.Message);
            }
        }
    }
}
