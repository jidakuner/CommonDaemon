using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerDaemon
{
    public static class Logger
    {
        public static string LogPath = string.Format("{0}{1}", Assembly.GetExecutingAssembly().Location,  ".log");

        public static void AppendLogForLog(string message, params string[] param)
        {
            AppendLog(string.Format(message, param), "LOG");
        }

        public static void AppendLogForWarning(string message, params string[] param)
        {
            AppendLog(string.Format(message, param), "WAR");
        }

        public static void AppendLogForError(string message, params string[] param)
        {
            AppendLog(string.Format(message, param), "ERR");
        }

        /// <summary>
        /// I don't think need huge log, so don't take care of perf.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public static void AppendLog(string message, string type)
        {
            using (StreamWriter write = File.AppendText(Logger.LogPath))
            {
                write.WriteLine(string.Format("[{0}]\t[{1}]\t{2}", DateTime.Now, type, message.Replace("\n", "\\n")));
            }
        }
    }
}
