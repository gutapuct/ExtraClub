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
using System.Windows.Shapes;
using TonusClub.Infrastructure.Interfaces;
using Microsoft.Win32;
using TonusClub.ServiceModel;
using TonusClub.UIControls;
using TonusClub.UIControls.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls.Club.Windows
{
    /// <summary>
    /// Interaction logic for UploadFileWindow.xaml
    /// </summary>
    public partial class UploadFileWindow
    {
        public Dictionary<int, string> FileTypes { get; set; }
        public object Forms { get; set; }

        private int _FileType;
        public int FileType
        {
            get
            {
                return _FileType;
            }
            set
            {
                _FileType = value;
                OnPropertyChanged("FileType");
                var vis = _FileType == 2 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                form.Visibility = vis;
                formtext.Visibility = vis;
            }
        }


        private Guid _Parameter;
        public Guid Parameter
        {
            get
            {
                return _Parameter;
            }
            set
            {
                _Parameter = value;
                OnPropertyChanged("Parameter");
            }
        }

        private string _FileName;
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
                OnPropertyChanged("FileName");
            }
        }

        public UploadFileWindow(ClientContext context):base(context)
        {
            InitializeComponent();
            Forms = context.GetCallScrenarioForms();
            FileTypes = new Dictionary<int, string>();
            FileTypes.Add(0, "Входящий звонок - новый клиент");
            FileTypes.Add(1, "Входящий звонок - старый клиент");
            FileTypes.Add(2, "Сценарий звонков");
            DataContext = this;
            FileType = 0;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_FileType == 2 && Parameter == Guid.Empty) return;
            try
            {
                _context.UploadFile(FileType, FileName, Parameter);
            }
            catch (Exception ex)
            {
                TonusWindow.Alert("Ошибка", "При загрузке файла произошла ошибка.\nУбедитесь, что файл не открыт в другой программе.\nЕсли ошибка повторяется несколько раз, обратитесь к разработчикам.\n\nСообщение об ошибке:\n" + ex.Message);
                return;
            }
            DialogResult = true;
            Close();
        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.Multiselect = false;
            if (dlg.ShowDialog() ?? false)
            {
                FileName = dlg.FileName;
            }
        }
    }
}
