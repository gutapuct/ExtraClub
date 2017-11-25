using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using TonusClub.Entities;
using TonusClub.ServerCore;
using TonusClub.ServiceModel;

namespace Flagmax.WorkflowService
{
    public class CloseDivisionUpdater
    {
        CancelThreadInfo _e;

        public CloseDivisionUpdater(CancelThreadInfo e)
        {
            _e = e;
        }

        static object _lock = new object();
        public Thread RunAsync()
        {
            var res = new Thread(new ThreadStart(Run));
            res.Start();
            return res;
        }

        private void Run()
        {
            lock (_lock)
            {
                try
                {
                    Logger.Log("Запуск обновления файлопомойки");
                    var res = new List<SshFileInfo>();
                    var res1 = new HashSet<string>();
                    using(var cli = new SftpClient("46.228.5.23", 22, "asu", "k7izwr9aDW"))
                    {
                        Logger.Log("Соединение с файлопомойкой установлено");

                        cli.Connect();
                        try
                        {
#if BEAUTINIKA
                            if (!_e.Cancel)
                            {
                                ReadDirectory(cli, "/home/sysadmin/fm/beautinika/business", res, res1);
                            }
                            if (!_e.Cancel)
                            {
                                ReadDirectory(cli, "/home/sysadmin/fm/beautinika/start", res, res1);
                            }
#else
                            if (!_e.Cancel)
                            {
                                ReadDirectory(cli, "/business", res, res1);
                            }
                            if (!_e.Cancel)
                            {
                                ReadDirectory(cli, "/start", res, res1);
                            }
                            if (!_e.Cancel)
                            {
                                ReadDirectory(cli, "/crysismaster", res, res1);
                            }
#endif
                        }
                        finally
                        {
                            cli.Disconnect();
                        }
                        Logger.Log("Список файлов из файлопомойке получен");
                    }
                    if (_e.Cancel) return;
                    using (var context = new TonusEntities())
                    {
                        foreach (var f in context.SshFiles.Select(i => new { i.Id, i.Path }).ToArray())
                        {
                            if (!res1.Contains(f.Path))
                            {
                                context.DeleteObject(context.SshFiles.Single(i => i.Id == f.Id));
                            }
                        }

                        foreach (var f in res)
                        {
                            if (_e.Cancel) return;
                            var sf = context.SshFiles.FirstOrDefault(i => i.Path == f.FullName);
                            if (sf == null)
                            {
                                context.SshFiles.AddObject(sf = new SshFile
                                {
                                    Id = Guid.NewGuid(),
                                    Path = f.FullName,
                                    Filename = f.Name
                                });
                            }
                            if (sf.Length != f.Length)
                            {
                                sf.Length = f.Length;
                            }
                            if (sf.ModifiedDate != f.Modified)
                            {
                                sf.ModifiedDate = f.Modified;
                            }
                        }
                        context.SaveChanges();
                    }
                    Logger.Log("Обновление файлопомойки завершено успешно");

                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }

        void ReadDirectory(SftpClient cli, string name, List<SshFileInfo> res, HashSet<string> res1)
        {
            if (_e.Cancel) return;
            //if (res.Count > 20) return;
            var fs = cli.ListDirectory(name);
            foreach (var i in fs)
            {
                try
                {
                    if (i.Name == "." || i.Name == "..") continue;
                    if (i.IsDirectory)
                    {
                        ReadDirectory(cli, i.FullName, res, res1);
                    }
                    else
                    {
                        if (!i.FullName.EndsWith("umbs.db") && !i.FullName.Contains("recycle_bin"))
                        {
                            res.Add(new SshFileInfo
                            {
                                FullName = i.FullName,
                                Length = i.Length,
                                Modified = i.LastWriteTimeUtc,
                                Name = i.Name
                            });
                            res1.Add(i.FullName);
                            Console.Write("*");
                        }
                    }
                }
                catch { }
            }

        }

    }
}
