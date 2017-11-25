using System;
using System.IO;

namespace UpdaterService
{
    public static class Logger
    {
        static readonly object LockObj = new object();

        public static void Log(Exception ex)
        {
            Log("---------------------------------------------");
            while (ex != null)
            {
                Log("Type: " + ex.GetType());
                Log("\r\nMessage: " + ex.Message);
                Log("\r\nStackTrace: " + ex.StackTrace);
                ex = ex.InnerException;
                if (ex != null) Log("\r\n\r\nInner exception:\r\n");
            }
        }

        public static void Log(string str)
        {
            lock (LockObj)
            {
                try
                {
                    var fold = new DirectoryInfo("c:\\temp\\");
                    if (!fold.Exists) fold.Create();
                    var f = new StreamWriter("c:\\temp\\tcupdater.log", true);
                    f.WriteLine($"{DateTime.Now}: {str}");
                    f.Close();
                }
                catch { }
            }
        }
    }

}
