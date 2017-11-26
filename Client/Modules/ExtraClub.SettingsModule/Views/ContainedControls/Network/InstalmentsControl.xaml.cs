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
using ExtraClub.SettingsModule.ViewModels;
using ExtraClub.SettingsModule.Views.ContainedControls.Network.Windows;
using Microsoft.Practices.Unity;
using ExtraClub.UIControls;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.BaseClasses;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Network
{
    public partial class InstalmentsControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public InstalmentsControl()
        {
            InitializeComponent();
        }

        private void InstalmentsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("FranchInstalmentsMgmt"))
            {
                EditButton_Click(null, null);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e1)
        {
            if (Model.OrgInstalmentsView.CurrentItem != null)
            {
                ExtraWindow.Confirm("Удаление", "Удалить выделенную рассрочку?", e =>
                {
                    if (e.DialogResult ?? false)
                    {
                        ClientContext.PostInstalmentDelete(((Instalment)Model.OrgInstalmentsView.CurrentItem).Id);
                        Model.RefreshInstalments();
                    }
                });
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.OrgInstalmentsView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditInstalmentWindow>(() => Model.RefreshInstalments(), new ParameterOverride("instalment", ViewModelBase.Clone<Instalment>(Model.OrgInstalmentsView.CurrentItem)));
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditInstalmentWindow>(() => Model.RefreshInstalments());
        }

        private void NewFolderClick(object sender, RoutedEventArgs e)
        {
            var id = Guid.Empty;
            if (Model.CurrentOrgInstalmentTreeItem != null && Model.CurrentOrgInstalmentTreeItem.Id != SettingsLargeViewModel.DeletedFolderId) id = Model.CurrentOrgInstalmentTreeItem.Id;
            ProcessUserDialog<NewEditFolderWindow>(() => Model.RefreshFolders(), new ResolverOverride[] { new ParameterOverride("parentId", id), new ParameterOverride("categoryId", 2) });
        }

        private void EditFolderClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentOrgInstalmentTreeItem != null && Model.CurrentOrgInstalmentTreeItem.Id != Guid.Empty && Model.CurrentOrgInstalmentTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ProcessUserDialog<NewEditFolderWindow>(() => Model.RefreshFolders(), new ResolverOverride[] { new ParameterOverride("folder", Model.CurrentOrgInstalmentTreeItem), new ParameterOverride("parentId", Guid.Empty), new ParameterOverride("categoryId", 2) });
            }
        }

        private void RemoveFolder_Click(object sender, RoutedEventArgs e1)
        {
            if (Model.CurrentOrgInstalmentTreeItem != null && Model.CurrentOrgInstalmentTreeItem.Id != Guid.Empty && Model.CurrentOrgInstalmentTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ExtraWindow.Confirm("Удаление папки",
                    "Удалить папку?\nСодержимое этой папки будут перемещено уровнем выше.", e =>
                {
                    if (e.DialogResult ?? false)
                    {
                        ClientContext.DeleteSettingsFolder(Model.CurrentOrgInstalmentTreeItem.Id);
                        Model.RefreshFolders();
                    }
                });
            }
        }
        private void Tree_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Model.CurrentOrgInstalmentTreeItem = (e.Source as Telerik.Windows.Controls.RadTreeView).SelectedItem as SettingsFolder;
        }
    }
}
