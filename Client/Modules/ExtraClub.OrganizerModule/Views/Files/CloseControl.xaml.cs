using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExtraClub.Infrastructure;
using ExtraClub.OrganizerModule.ViewModels;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.OrganizerModule.Views.Files
{
    public partial class CloseControl
    {
        private SshFilesViewModel Model
        {
            get
            {
                return DataContext as SshFilesViewModel;
            }
        }

        public CloseControl()
        {
            InitializeComponent();
            NavigationManager.DownloadFileRequest += NavigationManager_DownloadFileRequest;
        }

        void NavigationManager_DownloadFileRequest(object sender, GuidEventArgs e)
        {
            DownloadFile(ClientContext.GetSshFile(e.Guid));
        }

        private void EnqueueDownload(object sender, RoutedEventArgs e)
        {
            Model.Enqueue();
        }

        private void Download(object sender, RoutedEventArgs e)
        {
            if (Model.SshFilesView.CurrentItem != null)
            {
                var sf = Model.SshFilesView.CurrentItem as SshFile;
                //if (sf.Avail)
                {
                    DownloadFile(sf);
                }
            }
        }

        private void SshFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                Download(null, null);
            }
        }

        void DownloadFile(SshFile sf)
        {
            var dlg = new FolderBrowserDialog();
            dlg.Description = "Укажите путь для сохранения полученного файла";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var path = dlg.SelectedPath;
                //var fn = AppSettingsManager.GetSetting("ServerAddress").Replace("ExtraService.svc", "FileDownloadHandler.ashx") + "?file=" + sf.Id;
                var fp = System.IO.Path.Combine(path, sf.Filename);
                var d = new DownloadFileWindow(sf, fp);
                d.Start();
                d.Show();
            }

        }
    }
}
