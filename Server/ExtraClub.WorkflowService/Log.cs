using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ExtraClub.WorkflowService
{
    static class Log
    {
        static object lockObj = new object();
        public static void WriteLine(string line)
        {
            lock (lockObj)
            {
                try
                {
                    var sw = new StreamWriter("c:\\temp\\tcworkflow.log", true);
                    sw.WriteLine(line);
                    sw.Close();
                    sw.Dispose();
                }
                catch { }
            }
        }
    }
}
