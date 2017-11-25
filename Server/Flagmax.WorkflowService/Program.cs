using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Flagmax.WorkflowService.CommandLine;
using TonusClub.Entities;
using TonusClub.ServerCore;
// ReSharper disable LocalizableElement

namespace Flagmax.WorkflowService
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                if (args.Length == 0)
                {
                    DisplayHelp();
                    return;
                }
                if (args[0] == "-stream-creator")
                {
                    new StreamCreator().Run(args);
                }
                if (args[0] == "-tools")
                {
                    new Tools().Run(args);
                }
                if (args[0] == "-extract")
                {
                    new Extractor().Run(args);
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new FlagmaxWorkflow()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }


        private static void DisplayHelp()
        {
            Console.WriteLine("Использование:");
            Console.WriteLine();
            Console.WriteLine("Для создания дельты на стороне сервера:");
            Console.WriteLine("Flagmax.WorkflowService.exe -stream-creator <CompanyId> <Version> <OutputPath>");
            Console.WriteLine("Flagmax.WorkflowService.exe -stream-creator 91B751C9-B80A-4F27-A6DC-BFA5100E73C8  250100 c:\\temp");
            Console.WriteLine("Для первичной синхронизации необходимо указать версию -1.");
            Console.WriteLine();
            Console.WriteLine("Применение файла дельты:");
            Console.WriteLine("Flagmax.WorkflowService.exe -extract <Input file name> [-nocheck]");
            Console.WriteLine("-nocheck - не проводить проверку целостности данных, быстрая обработка, но необходимо отключить констрейнты в БД");
            Console.WriteLine();
            Console.WriteLine("Запуск полного цикла воркфлов:");
            Console.WriteLine("Flagmax.WorkflowService.exe -tools workflow");
            Console.WriteLine();
            Console.WriteLine("Автоматическе списание единиц для смарт-абонементов:");
            Console.WriteLine("Flagmax.WorkflowService.exe -tools smart <divisionId> <date>");
            Console.WriteLine("Flagmax.WorkflowService.exe -tools smart 91B751C9-B80A-4F26-A6DC-BFA5100E73C8 26.05.2015");

        }
    }
}