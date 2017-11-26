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
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Network.Windows
{
    public partial class SpreadWindow
    {
        public bool Maximum { get; set; }
        public bool Franch { get; set; }
        public bool Upravl { get; set; }
        public bool Admins { get; set; }

        public Permission Permission { get; set; }
        public SpreadWindow(ClientContext context, Permission permission):base(context)
        {
            InitializeComponent();
            Permission = permission;
            this.DataContext = this;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.SpreadPermission(Permission.PermissionId, Maximum, Franch, Upravl, Admins);
            DialogResult = true;
            Close();
        }
    }
}
