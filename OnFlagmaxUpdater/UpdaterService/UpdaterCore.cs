using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Specialized;
using System.Threading;
using Microsoft.SqlServer.Dac;

namespace UpdaterService
{
    public class UpdaterCore
    {
        private readonly RegistryKey _key;
        private readonly WebClient _client;
        private readonly string _wfPath, _wsPath;
        private readonly string _connectionString;

        private int WfVersion;
        private int ServerWfVersion;
        private int WsVersion;
        private int ServerWsVersion;
        private int DbVersion;
        private int ServerDbVersion;

        public UpdaterCore()
        {
            _key = Registry.LocalMachine.CreateSubKey("Software\\Extra\\");
            _client = new WebClient { Encoding = Encoding.UTF8 };

            _wfPath = _key.GetValue("WfPath", @"c:\program files\Extra\").ToString();
            _wsPath = _key.GetValue("WsPath", @"c:\inetpub\wwwroot\ws\").ToString();
            _connectionString = _key.GetValue("ConnectionString", "Data Source=localhost\\sqlexpress;Initial Catalog=ExtraClub;Persist Security Info=True;User ID=sa;Password=sa;").ToString();

            ServicePointManager.Expect100Continue = false;
        }

        public void Update(CancellationToken cancellationToken)
        {
            var path = new DirectoryInfo(Path.Combine(_wsPath, "bin"));

            Logger.Log("Paths received, initializing versions...");

            InitVersions();
            cancellationToken.ThrowIfCancellationRequested();

            Logger.Log($"Db: {DbVersion}/{ServerDbVersion}, Ws: {WsVersion}/{ServerWsVersion}, Wf: {WfVersion}/{ServerWfVersion}");

            if(!UpdatesAvailable())
            {
                return;
            }
            cancellationToken.ThrowIfCancellationRequested();

            if(!StopWorkflow())
            {
                throw new Exception("WF did not stop");
            }
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                ProcessDatabase();
            }
            catch(Exception ex)
            {
                if(!StartWorkflow())
                {
                    Logger.Log("WF did not start");
                }
                throw new Exception("DB update error, starting WF and exiting", ex);
            }
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                ProcessWebService();
            }
            catch(Exception ex)
            {
                if(DbVersion == ServerDbVersion)
                {
                    if(!StartWorkflow())
                    {
                        Logger.Log("WF did not start");
                    }
                }
                throw new Exception("WS update error, exiting", ex);
            }
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                ProcessWorkflow();
            }
            catch(Exception ex)
            {
                if(DbVersion == ServerDbVersion)
                {
                    if(!StartWorkflow())
                    {
                        Logger.Log("WF did not start");
                    }
                }
                throw new Exception("WF update error, exiting", ex);
            }
            cancellationToken.ThrowIfCancellationRequested();

            if(!StartWorkflow())
            {
                throw new Exception("WF did not start");
            }
            cancellationToken.ThrowIfCancellationRequested();

            Logger.Log("Update finished successfully");
        }

        private void InitVersions()
        {
            WfVersion = (int)_key.GetValue("WfVersion", -1);
            var serverVer = _client.DownloadString("http://asu.Extra.ru/Updater/GetCurrentWFVersion");
            ServerWfVersion = Int32.Parse(serverVer);

            WsVersion = (int)_key.GetValue("WsVersion", -1);
            serverVer = _client.DownloadString("http://asu.Extra.ru/Updater/GetCurrentWSVersion");
            ServerWsVersion = Int32.Parse(serverVer);

            DbVersion = (int)_key.GetValue("DbVersion", -1);
            serverVer = _client.DownloadString("http://asu.Extra.ru/Updater/GetCurrentDBVersionEx");
            ServerDbVersion = Int32.Parse(serverVer);
        }

        private bool UpdatesAvailable()
        {
            return DbVersion != ServerDbVersion || WsVersion != ServerWsVersion || WfVersion != ServerWfVersion;
        }

        private void ProcessWorkflow()
        {
            if(WfVersion == ServerWfVersion)
            {
                return;
            }

            var di = new DirectoryInfo(_wfPath);

            if(!di.Exists)
            {
                di.Create();
            }

            Unzip(_client.DownloadData("http://asu.Extra.ru/Updater/GetCurrentWF"), _wfPath);

            _key.SetValue("WfVersion", ServerWfVersion);

            ReportSuccess("Wf", ServerWfVersion);
        }

        private void ProcessWebService()
        {
            if(WsVersion == ServerWsVersion)
            {
                return;
            }

            var di = new DirectoryInfo(_wsPath);
            if(!di.Exists)
            {
                di.Create();
            }

            var wsBinDir = new DirectoryInfo(Path.Combine(_wsPath, "bin"));
            if(wsBinDir.Exists)
            {
                CleanDirectory(wsBinDir);
            }

            Unzip(_client.DownloadData("http://asu.Extra.ru/Updater/GetCurrentWS"), _wsPath);

            _key.SetValue("WsVersion", ServerWsVersion);

            ReportSuccess("Ws", ServerWsVersion);
        }

        private void CleanDirectory(DirectoryInfo directoryInfo)
        {
            try
            {
                Logger.Log($"Cleaning directory {directoryInfo.FullName}");

                foreach(FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }
                foreach(DirectoryInfo dir in directoryInfo.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch(Exception ex)
            {
                Logger.Log($"Error while cleaning {directoryInfo.FullName}");
                Logger.Log(ex);
            }
        }

        private void Unzip(byte[] content, string outFolder)
        {

            using(var zipInputStream = new ZipInputStream(new MemoryStream(content)))
            {
                var zipEntry = zipInputStream.GetNextEntry();

                while(zipEntry != null)
                {
                    var fullZipToPath = Path.Combine(outFolder, zipEntry.Name);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);

                    if(!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if(!zipEntry.IsDirectory)
                    {
                        using(var outputStream = File.Create(fullZipToPath))
                        {
                            zipInputStream.CopyTo(outputStream);
                        }
                    }

                    zipEntry = zipInputStream.GetNextEntry();
                }
            }
        }

        private void ProcessDatabase()
        {
            if(ServerDbVersion == DbVersion)
            {
                return;
            }

            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            Unzip(_client.DownloadData("http://asu.Extra.ru/Updater/GetCurrentDb"), tempPath);

            var package = DacPackage.Load(Path.Combine(tempPath, "ExtraClub.Database.dacpac"));

            var options = new DacDeployOptions
            {
                BlockOnPossibleDataLoss = false,
                CommandTimeout = 600000
            };

            var dbServices = new DacServices(_connectionString);
            dbServices.Message += DbServices_Message;

            dbServices.Deploy(package, "ExtraClub", true, options);

            _key.SetValue("DbVersion", ServerDbVersion);

            ReportSuccess("Db", ServerDbVersion);

            Directory.Delete(tempPath, true);
        }

        private void DbServices_Message(object sender, DacMessageEventArgs e)
        {
            Logger.Log($"{e.Message.Number} {e.Message.Prefix} {e.Message.Message}");
        }

        private bool StopWorkflow()
        {
            var service = GetService();
            if(service != null && !service.Status.Equals(ServiceControllerStatus.Stopped))
            {
                if(!service.Status.Equals(ServiceControllerStatus.StopPending))
                {
                    service.Stop();
                }
                try
                {
                    service.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 2, 0));
                    Thread.Sleep(30000);
                }
                catch(Exception ex)
                {
                    Logger.Log("Exception during service stop. Halt, then trying to resume.");
                    Logger.Log(ex);
                }

                if(service.Status != ServiceControllerStatus.Stopped)
                {
                    return false;
                }
            }
            return true;
        }

        private ServiceController GetService()
        {
            return ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "ExtraWorkflowService");
        }

        private bool StartWorkflow()
        {
            var service = GetService();
            if(service != null && !service.Status.Equals(ServiceControllerStatus.Running))
            {
                if(!service.Status.Equals(ServiceControllerStatus.StartPending))
                {
                    service.Start();
                }

                service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 8, 0));
                if(service.Status != ServiceControllerStatus.Running)
                {
                    return false;
                }
            }
            return true;
        }

        private void ReportSuccess(string component, object version)
        {
            var vals = new NameValueCollection();
            vals.Add("ClubId", GetClubId());
            vals.Add("Part", component);
            vals.Add("Version", version.ToString());

            _client.UploadValues("http://asu.Extra.ru/Updater/Success/", vals);
        }

        private string GetClubId()
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP 1 DefaultDivisionId FROM LocalSettings";
                cmd.CommandType = System.Data.CommandType.Text;

                return Convert.ToString(cmd.ExecuteScalar());
            }
        }
    }
}
