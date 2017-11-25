using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Management;

namespace HWKeyGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GetSystemId());
            Console.ReadKey();
        }

        public static string GetSystemId()
        {
            var sysInfo = new StringBuilder();
            var mc = new ManagementClass("Win32_Processor");
            var moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                sysInfo.Append(mo.Properties["ProcessorId"].Value.ToString());
            }

            sysInfo.Append(Environment.MachineName);

            //mc = new ManagementClass("Win32_DiskDrive");
            //moc = mc.GetInstances();
            //foreach (ManagementObject mo in moc)
            //{
            //    if (sysInfo != null && mo != null && mo.Properties != null && mo.Properties["SerialNumber"] != null && mo.Properties["SerialNumber"].Value != null)
            //        sysInfo.Append(mo.Properties["SerialNumber"].Value.ToString());
            //}

            return CalculateSHA1(sysInfo.ToString());
        }


        public static string CalculateSHA1(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            string hash = BitConverter.ToString(
                cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");

            return hash;
        }
    }
}
