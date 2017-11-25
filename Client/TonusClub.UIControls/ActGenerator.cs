using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using TonusClub.Infrastructure;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Interfaces;

namespace TonusClub.UIControls
{
    public static class ActGenerator
    {
        public static void GenerateAct(ClientContext context, IReportManager manager)
        {
            var pars = new Dictionary<string, string>();
            var sb = new StringBuilder();
            foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (sb.Length > 0) sb.Append("; ");
                sb.Append(ip);
            }
            pars.Add("LocalIPs", sb.ToString());
            pars.Add("ServerAddr", AppSettingsManager.GetSetting("ServerAddress").Replace("/TonusService.svc", "").Replace("https://", ""));
            pars.Add("Franch", context.CurrentCompany.CompanyName);
            pars.Add("Club", context.CurrentDivision.Name);
            pars.Add("Windows", $"{Environment.OSVersion.Platform} {Environment.OSVersion.Version}");
            pars.Add("Is64", Environment.Is64BitOperatingSystem ? "Да" : "Нет");
            pars.Add("UserName", Environment.UserName);
            pars.Add("Now", DateTime.Now.ToString());
            pars.Add("Locale", Thread.CurrentThread.CurrentCulture.Name);

            HardwareConfigWriter.AddMachineConfig(pars, "Client");

            manager.ProcessPdfReport(()=>context.GenerateFirstRun(pars.ToArray()));
        }
    }
}
