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
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows
{
    public partial class NewEditCorporateWindow
    {
        public Corporate Corporate { get; set; }
        public List<CompanySettingsFolder> SettingsFolders { get; set; }

        public NewEditCorporateWindow(ClientContext context, Corporate corporate)
            : base(context)
        {
            SettingsFolders = context.GetCompanySettingsFolders(2);
            Owner = Application.Current.MainWindow;

            if (corporate.Id == Guid.Empty)
            {
                Corporate = new Corporate
                {
                    IsAvail = true,
                    Name = "Новый корпоративный договор",
                    SettingsFolderId = null
                };
            }
            else
            {
                Corporate = corporate;
            }
            InitializeComponent();
            DataContext = this;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_context.PostCorporate(Corporate.Id, Corporate.Name, Corporate.SettingsFolderId))
            {
                DialogResult = true;
                Close();
            }
        }
    }
}
