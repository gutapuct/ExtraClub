using System;
using System.ServiceProcess;

namespace UpdaterService
{
    static class Program
    {
        static void Main()
        {
            if(Environment.UserInteractive)
            {
                try
                {
                    new UpdaterCore().Update(new System.Threading.CancellationToken());
                }
                catch(Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new ExtraUpdaterService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
