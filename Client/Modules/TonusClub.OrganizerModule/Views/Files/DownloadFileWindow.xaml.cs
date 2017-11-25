using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TonusClub.Infrastructure;

namespace TonusClub.OrganizerModule.Views.Files
{
    /// <summary>
    /// Interaction logic for DownloadFileWindow.xaml
    /// </summary>
    public partial class DownloadFileWindow : Window
    {
        private string FileName;
        private string Destination;

        SftpClient client;
        BackgroundWorker bw;

        public DownloadFileWindow(ServiceModel.SshFile sf, string fp)
        {
            FileName = sf.Path;
            Destination = fp;
            InitializeComponent();

            text.Text = sf.Filename;

            client = new SftpClient("46.228.5.23", 22, "tcaccount", "cr@m7sTe");
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        }

        public void Start()
        {
            bw.RunWorkerAsync();
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                Logger.Log(e.Error);
                MessageBox.Show("Ошибка при загрузке файла!\n" + e.Error.Message);
                return;
            }
            if(!e.Cancelled)
            {
                Process.Start(Destination);
            }
            else
            {
                try
                {
                    File.Delete(Destination);
                }
                catch { }
            }
            Close();
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if(progr != null)
            {
                progr.Value = e.ProgressPercentage;
            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            client.Connect();
            try
            {
                var f = client.OpenRead(FileName);
                try
                {
                    var file = new FileInfo(Destination);
                    if(file.Exists)
                    {
                        file.Delete();
                    }
                    var fs = file.Create();
                    var buff = new byte[65536];
                    var read = 0;

                    var len = f.Length;

                    while((read = f.Read(buff, 0, buff.Length)) > 0)
                    {
                        fs.Write(buff, 0, read);
                        bw.ReportProgress((int)Math.Round((float)f.Position / f.Length * 100));
                        if(e.Cancel)
                        {
                            break;
                        }
                    }
                    fs.Close();
                    if(e.Cancel)
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
                client.Disconnect();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            bw.CancelAsync();
            base.OnClosing(e);
        }
    }
}
