using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Practices.Unity;
using ExtraClub.SettingsModule.ViewModels;
using ExtraClub.SettingsModule.Views.ContainedControls.Club.Windows;
using ExtraClub.ServiceModel;
using System.Diagnostics;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Club
{
    /// <summary>
    /// Interaction logic for FilesControl.xaml
    /// </summary>
    public partial class FilesControl
    {
        public SettingsLargeViewModel Model
        {
            get
            {
                return DataContext as SettingsLargeViewModel;
            }
        }
        public FilesControl()
        {
            InitializeComponent();
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<UploadFileWindow>(() => Model.RefreshFiles());
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var file = Model.FilesView.CurrentItem as File;
            if (file == null) return;
            //TODO: deletion of common files
            ExtraWindow.Confirm("Удаление файла", "Вы действительно хотите удалить выделенный файл?\nВосстановление будет невозможно!", wnd =>
            {
                if (wnd.DialogResult ?? false)
                {
                    ClientContext.DeleteObject("Files", file.Id);
                    Model.RefreshFiles();
                }
            });
        }

        private void FilesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                var file = Model.FilesView.CurrentItem as File;
                Process.Start(ClientContext.DownloadFile(file));
            }
        }
    }
}
