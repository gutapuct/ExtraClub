using System;
using System.IO;
using System.Linq;
using ExtraClub.ServerCore;

namespace ExtraClub.WorkflowService.CommandLine
{
    internal sealed class Extractor
    {
        public void Run(string[] args)
        {
            return;
            if (!File.Exists(args[1]))
            {
                Console.WriteLine(@"Указанный файл не найден!");
                return;
            }
            using (var fs = new FileStream(args[1], FileMode.Open))
            {
                if (args.Contains("-nocheck"))
                {
                    new SyncCoreEx().CommitStreamEx(fs);
                }
                else
                {
                    string str = "";
                    SyncCore.CommitStream(fs, ref str);
                }
            }
        }
    }
}
