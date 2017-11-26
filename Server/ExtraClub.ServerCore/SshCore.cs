using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ExtraClub.ServiceModel;

namespace ExtraClub.ServerCore
{
    static class SshCore
    {
        internal static bool DownloadFile(SshFile fi)
        {
            return DownloadFile(fi, new CancelThreadInfo());
        }

        internal static bool DownloadFile(SshFile fi, CancelThreadInfo e)
        {
            Logger.Log("Загрузка файла " + fi.Path);
            using(var cli = new SftpClient("46.228.5.23", 22, "asu", "k7izwr9aDW"))
            {
                try
                {
                    cli.Connect();
                    try
                    {
                        var f = cli.OpenRead(fi.Path);
                        try
                        {
                            var file = new FileInfo(Path.Combine("c:\\temp\\", fi.Id.ToString()));
                            if (file.Exists)
                            {
                                file.Delete();
                            }
                            var fs = file.Create();
                            var buff = new byte[65536];
                            var read = 0;
                            while ((read = f.Read(buff, 0, buff.Length)) > 0)
                            {
                                fs.Write(buff, 0, read);
                                if (e.Cancel)
                                {
                                    break;
                                }
                            }
                            fs.Close();
                            if (e.Cancel)
                            {
                                file.Delete();
                            }
                        }
                        finally
                        {
                            f.Close();
                        }
                    }
                    finally
                    {
                        cli.Disconnect();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    return false;
                }
                return true;
            }
        }

    }
}
