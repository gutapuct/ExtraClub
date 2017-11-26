using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ExtraClub.Infrastructure
{
    public static class Logger
    {
        static object _lockObj = new object();

        public static void Log(string str, params object[] parameters)
        {
            lock (_lockObj)
            {
                try
                {
                    var fold = new DirectoryInfo("c:\\temp\\");
                    if (!fold.Exists) fold.Create();
                    var f = new StreamWriter("c:\\temp\\tcclient.log", true);
                    f.WriteLine(String.Format(str, parameters));
                    f.Close();
                }
                catch { }
            }
        }

        public static void Log(Exception ex)
        {
            Log("----------------------------------");
            Log(DateTime.Now.ToString());
            while (ex != null)
            {
                Log("Type: " + ex.GetType().ToString());
                Log("Message: " + ex.Message);
                Log("Trace: " + ex.StackTrace);
                ex = ex.InnerException;
                if (ex != null) Log("\nInner exception:\n");
            }
        }
    }
}
