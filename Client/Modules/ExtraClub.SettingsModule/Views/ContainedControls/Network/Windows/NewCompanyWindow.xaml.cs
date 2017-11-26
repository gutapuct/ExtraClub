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
using ExtraClub.ServiceModel;
using System.ServiceModel;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Network.Windows
{
    /// <summary>
    /// Interaction logic for NewCompanyWindow.xaml
    /// </summary>
    public partial class NewCompanyWindow
    {
        public string CompName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ReportEmail { get; set; }
        public Guid RoleId { get; set; }
        public int UtcCorr { get; set; }
        public string UserPrefix { get; set; }

        public List<Role> Roles { get; set; }

        public NewCompanyWindow(ClientContext context) : base(context)
        {
            InitializeComponent();
            Roles = context.GetRoles();
            UtcCorr = 0;
            DataContext = this;
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(CompName) || String.IsNullOrEmpty(Login) || String.IsNullOrEmpty(Password) || RoleId == Guid.Empty || String.IsNullOrWhiteSpace(UserPrefix)) return;

            try
            {
                _context.PostNewCompany(CompName, Login, Password, RoleId, ReportEmail, UtcCorr, UserPrefix);
            }
            catch (FaultException exc)
            {
                ExtraWindow.Alert("Ошибка", exc.Message);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
