using Microsoft.Practices.Unity;
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
using TonusClub.ServiceModel;
using TonusClub.SettingsModule.Views.ContainedControls.Network.Windows;
using TonusClub.UIControls;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows
{
    /// <summary>
    /// Interaction logic for NewEditRoleWindow.xaml
    /// </summary>
    public partial class NewEditRoleWindow
    {
        private Guid Id { get; set; }
        public string RoleName { get; set; }
        public string TicketDisc { get; set; }
        public string TicketRubDisc { get; set; }
        public string CardDisc { get; set; }
        List<Permission> allPerms;
        public List<Permission> Permissions { get; set; }
        public Guid? SettingsFolderId { get; set; }
        public List<CompanySettingsFolder> SettingsFolders { get; set; }
        public Visibility IsSpreadVisible { get; set; }
        IUnityContainer _container;

        public NewEditRoleWindow(ClientContext context, Role role, IUnityContainer container)
            : base(context)
        {
            _container = container;
            allPerms = context.GetAllPermissions();
            if (context.CheckPermission("SyncAdmin"))
            {
                IsSpreadVisible = System.Windows.Visibility.Visible;
            }
            else
            {
                IsSpreadVisible = System.Windows.Visibility.Collapsed;
            }
            Permissions = allPerms.Where(i => !i.ParentPermissionId.HasValue).ToList();
            SettingsFolders = context.GetCompanySettingsFolders(6);

            if (role.RoleId == Guid.Empty)
            {
                Id = Guid.Empty;
            }
            else
            {
                Id = role.RoleId;
                RoleName = role.RoleName;
                CardDisc = role.CardDiscs;
                TicketDisc = role.TicketDiscs;
                TicketRubDisc = role.TicketRubDiscs;
                SettingsFolderId = role.SettingsFolderId;
                allPerms.Where(i => role.SerializedPermissions.Contains(i.PermissionId)).ToList().ForEach(i => i.Helper = true);
            }
            InitializeComponent();
            DataContext = this;
            TreeView.ExpandAll();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostRole(Id, RoleName, allPerms.Where(i => i.Helper/* ?? false*/).Select(i => i.PermissionId).ToArray(), CardDisc, TicketDisc, TicketRubDisc, SettingsFolderId);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SpreadButtonClick(object sender, RoutedEventArgs e)
        {
            ModuleViewBase.ProcessUserDialog<SpreadWindow>(_container, () => { }, new ParameterOverride("permission", ((FrameworkElement)sender).DataContext));
        }
    }
}
