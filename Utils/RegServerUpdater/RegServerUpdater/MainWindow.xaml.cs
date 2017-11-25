using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Net;
using System.Net.Security;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace RegServerUpdater
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string ServerFiles { get; set; }
        public string SqlAddr { get; set; }
        public string SqlLogin { get; set; }
        public string SqlPassword { get; set; }
        public string Log { get; set; }

        public MainWindow()
        {
            ServerFiles = "c:\\inetpub\\wwwroot\\ws";
            SqlAddr = "localhost\\sqlexpress";
            SqlLogin = "sa";
            SqlPassword = "sa";
            DataContext = this;
            InitializeComponent();
        }

        private void TestConnection(object sender, RoutedEventArgs e)
        {
            var conn = new SqlConnection(String.Format("Data Source={0};Initial Catalog=TonusClub;User Id={1};Password={2};", SqlAddr, SqlLogin, SqlPassword));
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("Все в порядке");
        }

        private void StartPatch(object sender, RoutedEventArgs e)
        {
            var conn = new SqlConnection(String.Format("Data Source={0};Initial Catalog=TonusClub;User Id={1};Password={2};", SqlAddr, SqlLogin, SqlPassword));
            conn.Open();
            var trans = conn.BeginTransaction();
            try
            {
                PatchDB(conn, trans);
                trans.Commit();
            }
            catch
            {
                trans.Rollback();
            }
            conn.Close();
            PatchServer();
            log("Завершено");
        }

        private void PatchServer()
        {
            log("Загрузка и замена файлов сервиса");
            var filelist = new List<string>
            {
                "TonusClub.Entities.dll",
                "TonusClub.ServerCore.dll",
                "TonusClub.ServerDataModule.dll",
                "TonusClub.ServiceModel.dll",
                "TonusClub.Entities.pdb",
                "TonusClub.ServerCore.pdb",
                "TonusClub.ServerDataModule.pdb",
                "TonusClub.ServiceModel.pdb",
                "App_Code.dll"
            };
            var wc = new WebClient();
            var flag = true;
            try
            {
                foreach (var fi in filelist)
                {
                    wc.DownloadFile("ftp://176.9.79.174/WS/" + fi, "c:\\temp\\" + fi);
                }
            }
            catch
            {
                log("Ошибка при загрузке файлов");
                flag = false;
            }
            if (flag)
            {
                var p = Process.Start(new ProcessStartInfo("iisreset", "/stop"));
                while (!p.HasExited)
                {
                    Thread.Sleep(100);
                }
                log("IIS остановлен");

                try
                {
                    foreach (var fi in filelist)
                    {
                        log("Замена файла " + fi + "...");
                        File.Delete(Path.Combine(ServerFiles, "bin", fi));
                        File.Move("c:\\temp\\" + fi, Path.Combine(ServerFiles, "bin", fi));
                    }
                }
                finally
                {
                    p = Process.Start(new ProcessStartInfo("iisreset", "/start"));
                    while (!p.HasExited)
                    {
                        Thread.Sleep(100);
                    }
                    log("IIS запущен");
                }
            }
        }

        private void PatchDB(SqlConnection conn, SqlTransaction trans)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "Select [DbVersion] from [Localsettings]";
            cmd.Transaction = trans;
            var localVer = (int)cmd.ExecuteScalar();
            log("локальная версия БД: " + localVer.ToString());

            log("поиск новых версий...");
            var wc = new WebClient();
            var flag = true;
            var currentlyGetting = localVer+1;
            var patches = new List<string>();
            while (flag)
            {
                try
                {
                    patches.Add(wc.DownloadString("ftp://176.9.79.174/Scripts/chscript" + currentlyGetting.ToString() + ".sql"));
                    currentlyGetting++;
                }
                catch {
                    flag = false;
                }
            }
            log("Всего скриптов загружено: " + patches.Count);
            var i = 1;
            foreach (var patch in patches)
            {
                log("Выполняется скрипт " + i++);
                var cmd1 = new SqlCommand
                {
                    Connection = conn,
                    Transaction = trans,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = patch
                };
                cmd1.ExecuteScalar();
            }
            log("Все скрипты применены, версия базы - последняя");
        }

        private void log(string p)
        {
            Log += p + "\n";
            OnPropertyChanged("Log");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
