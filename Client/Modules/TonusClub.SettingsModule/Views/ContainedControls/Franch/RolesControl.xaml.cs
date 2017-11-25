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
using TonusClub.SettingsModule.ViewModels;
using TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows;
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure.BaseClasses;
using TonusClub.ServiceModel;
using TonusClub.UIControls.BaseClasses;
using TonusClub.UIControls.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch
{
    public partial class RolesControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public RolesControl()
        {
            InitializeComponent();
            this.GotFocus += new RoutedEventHandler(OrgCardTypesControl_GotFocus);
        }

        void OrgCardTypesControl_GotFocus(object sender, RoutedEventArgs e)
        {
            Model.UpdateFinished -= new EventHandler(Model_UpdateFinished);
            Model.UpdateFinished += new EventHandler(Model_UpdateFinished);
        }

        void Model_UpdateFinished(object sender, EventArgs e)
        {
            if (RoleTree != null)
            {
                RoleTree.ExpandAll();
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditRoleWindow>(() => Model.RefreshRoles());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.RolesView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditRoleWindow>(() => Model.RefreshRoles(), new ParameterOverride("role", ViewModelBase.Clone<Role>(Model.RolesView.CurrentItem)));
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.RolesView.CurrentItem == null) return;
            TonusWindow.Confirm("Удаление роли", "Вы действительно хотите удалить выделенную роль?", w =>
            {
                if (w.DialogResult ?? false)
                {
                    ClientContext.DeleteRole((Model.RolesView.CurrentItem as Role).RoleId);
                    Model.RefreshRoles();
                }
            });
        }

        private void RolesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("FranchRolesMgmt"))
            {
                EditButton_Click(null, null);
            }
        }

        private void RoleTree_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Model.CurrentRoleTreeItem = (e.Source as Telerik.Windows.Controls.RadTreeView).SelectedItem as CompanySettingsFolder;
        }

        private void NewFolderClick(object sender, RoutedEventArgs e)
        {
            var id = Guid.Empty;
            if (Model.CurrentRoleTreeItem != null && Model.CurrentRoleTreeItem.Id != SettingsLargeViewModel.DeletedFolderId) id = Model.CurrentRoleTreeItem.Id;
            ProcessUserDialog<NewEditCompanyFolderWindow>(() => Model.RefreshCompanyFolders(), new ParameterOverride("parentId", id), new ParameterOverride("categoryId", 6));
        }

        private void EditFolderClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentRoleTreeItem != null && Model.CurrentRoleTreeItem.Id != Guid.Empty && Model.CurrentRoleTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ProcessUserDialog<NewEditCompanyFolderWindow>(() => Model.RefreshCompanyFolders(),
                    new ParameterOverride("folder", Model.CurrentRoleTreeItem),
                    new ParameterOverride("parentId", Guid.Empty),
                    new ParameterOverride("categoryId", 6));
            }
        }

        private void RemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentRoleTreeItem != null && Model.CurrentRoleTreeItem.Id != Guid.Empty && Model.CurrentRoleTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                TonusWindow.Confirm(
                    "Удаление папки",
                    "Удалить папку?\nСодержимое этой папки будут перемещено уровнем выше.", w =>
                {
                    if (w.DialogResult ?? false)
                    {
                        ClientContext.DeleteSettingsFolder(Model.CurrentRoleTreeItem.Id);
                        Model.RefreshCompanyFolders();
                    }
                });
            }
        }
    }
}