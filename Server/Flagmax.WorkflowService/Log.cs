using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Flagmax.WorkflowService
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
#if BEAUTINIKA
                    var sw = new StreamWriter("c:\\temp\\beautinikaworkflow.log", true);
#else
                    var sw = new StreamWriter("c:\\temp\\tcworkflow.log", true);
#endif
                    sw.WriteLine(line);
                    sw.Close();
                    sw.Dispose();
                }
                catch { }
            }
        }
    }
}
