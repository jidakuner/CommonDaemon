using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CommonDaemon
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive && args.Length > 0)
            {
                //run setup
                if (IsRunningAsAdministrator())
                {
                    string parameter = args[0];
                    switch (parameter)
                    {
                        case "-i":
                            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                            Logger.AppendLogForLog("Install service: \"{0}\"", Assembly.GetExecutingAssembly().Location);
                            break;
                        case "-u":
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                            Logger.AppendLogForLog("Uninstall service: \"{0}\"", Assembly.GetExecutingAssembly().Location);
                            break;
                    }
                }
                else
                {
                    ReEnterAsAdministrator(args);
                }


            }
            else
            {

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new CommonDaemon()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }


        static void ReEnterAsAdministrator(string[] args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                Arguments = string.Join(" ", args),
                Verb = "runas"
            };


            try
            {
                Process proc = new Process
                {
                    EnableRaisingEvents = true,
                    StartInfo = startInfo
                };
                proc.Start();
                proc.WaitForExit();
            }
            catch
            {
                return;
            }
        }

        static bool IsRunningAsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
