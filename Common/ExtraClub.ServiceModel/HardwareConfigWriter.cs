using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace ExtraClub.ServiceModel
{
    public static class HardwareConfigWriter
    {
        public static void AddMachineConfig(Dictionary<string, string> pars, string prefix)
        {
            pars.Add(prefix + "CpuNumber", CpuNumber());
            pars.Add(prefix + "CpuSpeed", CPUSpeed().ToString());
            pars.Add(prefix + "Memory", GetMemory());
            pars.Add(prefix + "Hdds", GetHdds());
        }

        public static string CpuNumber()
        {
            ManagementObjectSearcher mgmtObjects =
                new ManagementObjectSearcher("Select * from Win32_ComputerSystem");

            foreach (var item in mgmtObjects.Get())
            {
                return item["NumberOfLogicalProcessors"].ToString();
            }
            return "Неизвестно";
        }

        public static uint CPUSpeed()
        {
            ManagementObject Mo = new ManagementObject("Win32_Processor.DeviceID='CPU0'");
            uint sp = (uint)(Mo["CurrentClockSpeed"]);
            Mo.Dispose();
            return sp;
        }

        public static string GetMemory()
        {
            ObjectQuery winQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(winQuery);

            foreach (ManagementObject item in searcher.Get())
            {
                return (((ulong)item["TotalVisibleMemorySize"])/1024/1024).ToString("n2");
            }
            return "Неизвестно";
        }

        public static string GetHdds()
        {
            ManagementObjectSearcher mosDisks = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            var sb = new StringBuilder();
            foreach (ManagementObject moDisk in mosDisks.Get())
            {
                sb.Append("Модель: ");
                sb.AppendLine(moDisk["Model"].ToString());
                sb.Append("Размер, гб: ");
                sb.AppendLine(Math.Round(((((double)Convert.ToDouble(moDisk["Size"]) / 1024) / 1024) / 1024), 2).ToString());
                sb.AppendLine("Интерфейс: " + moDisk["InterfaceType"].ToString());
                sb.AppendLine();
            }
            sb.Replace("\n", "<br>");
            return sb.ToString();
        }
    }
}
