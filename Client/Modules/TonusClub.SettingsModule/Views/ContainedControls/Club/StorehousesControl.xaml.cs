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
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure.BaseClasses;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Windows;
using Telerik.Windows.Controls;
using TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows;
using ViewModelBase = TonusClub.UIControls.BaseClasses.ViewModelBase;

namespace TonusClub.SettingsModule.Views.ContainedControls.Club
{
    /// <summary>
    /// Interaction logic for StorehousesControl.xaml
    /// </summary>
    public partial class StorehousesControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public StorehousesControl()
        {
            InitializeComponent();
        }

        private void NewStorehouseButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditStorehouseWindow>(() => Model.RefreshStorehouses());
        }

        private void EditStorehouseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.StorehousesView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditStorehouseWindow>(() => Model.RefreshStorehouses(),
                    new ParameterOverride("storehouse", ViewModelBase.Clone<Storehouse>(Model.StorehousesView.CurrentItem)));
            }
        }

        private void DeleteStorehouseButton_Click(object sender, RoutedEventArgs e)
        {
            //if (Model.StorehousesView.CurrentItem == null) return;
            //TonusWindow.Confirm(new DialogParameters
            //{
            //    Header = "Удаление",
            //    Content = "Удалить выделенный склад?",
            //    Closed = delegate(object sender1, WindowClosedEventArgs e1)
            //    {
            //        if ((e1.DialogResult ?? false))
            //        {
            //            ClientContext.DeleteObject("Storehouses", ((Storehouse)Model.StorehousesView.CurrentItem).Id);
            //            Model.RefreshStorehouses();
            //        }
            //    }
            //});
        }

        private void StorehousesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("ClubStoresMgmt"))
            {
                EditStorehouseButton_Click(null, null);
            }
        }

        private void NewFolderClick(object sender, RoutedEventArgs e)
        {
            var id = Guid.Empty;
            if (Model.CurrentStoreTreeItem != null && Model.CurrentStoreTreeItem.Id != SettingsLargeViewModel.DeletedFolderId) id = Model.CurrentStoreTreeItem.Id;
            ProcessUserDialog<NewEditCompanyFolderWindow>(
                ()=>Model.RefreshCompanyFolders(), new ResolverOverride[] { new ParameterOverride("parentId", id), new ParameterOverride("categoryId", 5) });
        }

        private void EditFolderClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentStoreTreeItem != null && Model.CurrentStoreTreeItem.Id != Guid.Empty && Model.CurrentStoreTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ProcessUserDialog<NewEditCompanyFolderWindow>(
                    ()=>Model.RefreshCompanyFolders(),
                    new ResolverOverride[] { new ParameterOverride("folder", Model.CurrentStoreTreeItem), new ParameterOverride("parentId", Guid.Empty), new ParameterOverride("categoryId", 5) });
            }
        }

        private void RemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentStoreTreeItem != null && Model.CurrentStoreTreeItem.Id != Guid.Empty && Model.CurrentStoreTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                TonusWindow.Confirm("Удаление папки", "Удалить папку?\nСодержимое этой папки будут перемещено уровнем выше.", w =>
                {
                    if (w.DialogResult ?? false)
                    {
                        ClientContext.DeleteSettingsFolder(Model.CurrentStoreTreeItem.Id);
                        Model.RefreshCompanyFolders();
                    }
                });
            }
        }

        private void StoreTree_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Model.CurrentStoreTreeItem = (e.Source as Telerik.Windows.Controls.RadTreeView).SelectedItem as CompanySettingsFolder;
        }
    }
}
