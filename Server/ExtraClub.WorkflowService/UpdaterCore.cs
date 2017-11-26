using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.Collections.Specialized;
using ExtraClub.ServerCore;

namespace ExtraClub.WorkflowService
{
    public class UpdaterCore
    {
        readonly RegistryKey Key;
        readonly WebClient Client;
        readonly string UpdPath;
        int UpdVersion;
        int ServerUpdVersion;

        string ClubId;

        public UpdaterCore(Guid clubId)
        {
            ClubId = clubId.ToString();
            Key = Registry.LocalMachine.CreateSubKey("Software\\ExtraClub\\");
            Client = new WebClient { Encoding = Encoding.UTF8 };
            UpdPath = Key.GetValue("UpdPath", @"c:\program files\ExtraClubupdater\").ToString();
        }

        public bool Update()
        {
            try
            {
                //Logger.Log(String.Format("{0}: Paths received, initializing versions", DateTime.Now));
                InitVersions();

                //Logger.Log(String.Format("{0}: Upd: {1}/{2}", DateTime.Now, UpdVersion, ServerUpdVersion));
                if (!UpdatesAvailable()) return true;

                if (!StopUpdater())
                {
                    Logger.Log("Updater did not stop");
                    return false;
                }

                if (!ProcessUpdater())
                {
                    Logger.Log("Updater update error, exiting");
                    return false;
                }

                if (!StartUpdater())
                {
                    Logger.Log("Updater did not start");
                    return false;
                }

                Logger.Log("Update finished successfully");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("\r\nException during update!");
                Logger.Log(ex);
                return false;
            }
        }

        private void InitVersions()
        {
            UpdVersion = (int)Key.GetValue("UpdVersion", -1);
            var serverVer = Client.DownloadString("http://asu.ExtraClub.ru/Updater/GetCurrentUpdVersion");
            ServerUpdVersion = Int32.Parse(serverVer);
        }

        private bool UpdatesAvailable()
        {
            return UpdVersion != ServerUpdVersion;
        }

        private bool ProcessUpdater()
        {
            var di = new DirectoryInfo(UpdPath);
            if (!di.Exists)
            {
                Logger.Log("Upd directory " + UpdPath + " does not exist");
                return false;
            }

            if (UpdVersion != ServerUpdVersion)
            {
                var content = Client.DownloadData("http://asu.ExtraClub.ru/Updater/GetCurrentUpd");
                UnzipFromStream(new MemoryStream(content), UpdPath);
                Key.SetValue("UpdVersion", ServerUpdVersion);
            }
            System.Net.ServicePointManager.Expect100Continue = false;
            var vals = new NameValueCollection();
            vals.Add("ClubId", ClubId);
            vals.Add("Part", "Upd");
            vals.Add("Version", ServerUpdVersion.ToString());
            Client.UploadValues("http://asu.ExtraClub.ru/Updater/Success/", vals);

            return true;
        }

        private void UnzipFromStream(Stream zipStream, string outFolder)
        {

            var zipInputStream = new ZipInputStream(zipStream);
            var zipEntry = zipInputStream.GetNextEntry();
            while (zipEntry != null)
            {
                var entryFileName = zipEntry.Name;
                var buffer = new byte[4096];
                var fullZipToPath = Path.Combine(outFolder, entryFileName);
                var directoryName = Path.GetDirectoryName(fullZipToPath);
                if (!string.IsNullOrEmpty(directoryName))
                    Directory.CreateDirectory(directoryName);

                if (!zipEntry.IsDirectory)
                {
                    using (var streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipInputStream, streamWriter, buffer);
                    }
                }
                zipEntry = zipInputStream.GetNextEntry();
            }
        }

        private bool StopUpdater()
        {
            var service = new ServiceController("ExtraClubUpdaterService");
            if (!service.Status.Equals(ServiceControllerStatus.Stopped))
            {
                if (!service.Status.Equals(ServiceControllerStatus.StopPending))
                {
                    service.Stop();
                }
                service.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 1, 0));
                if (service.Status != ServiceControllerStatus.Stopped)
                {
                    return false;
                }
            }
            return true;
        }

        private bool StartUpdater()
        {
            var service = new ServiceController("ExtraClubUpdaterService");
            if (!service.Status.Equals(ServiceControllerStatus.Running))
            {
                if (!service.Status.Equals(ServiceControllerStatus.StartPending))
                {
                    service.Start();
                }
                service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 1, 0));
                if (service.Status != ServiceControllerStatus.Running)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
