using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewtonicServer.Server;
using System.IO;

namespace NewtonicServer
{
    static class Logger
    {
        public static event EventHandler<StringEventArgs> OnLog;

        public static void Log(string clientName, string str)
        {
            WriteFile(clientName, str);
            if (OnLog != null)
            {
                OnLog(clientName, new StringEventArgs { String = str });
            }
        }

        public static void Log(string message)
        {
            WriteFile("Система", message);
            if (OnLog != null)
            {
                OnLog("Система", new StringEventArgs { String = message });
            }
        }

        static void WriteFile(string clientName, string str)
        {
            try
            {
                var sw = new StreamWriter("c:\\temp\\newtonicsrv.log", true);
                sw.WriteLine(String.Format("[{0:H:mm:ss}] {1} : {2}", DateTime.Now, clientName, str));
                sw.Close();
                sw.Dispose();
            }
            catch { }
        }
    }
}
