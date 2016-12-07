using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CommonDaemon
{
    public static class Logger
    {
        public static string LogPath = string.Format("{0}{1}", System.AppDomain.CurrentDomain.BaseDirectory, "CommonDaemon.log");

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
