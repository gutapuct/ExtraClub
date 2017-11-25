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
using TonusClub.UIControls.Windows;
using TonusClub.SettingsModule.ViewModels;
using TonusClub.ServiceModel;
using TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows;
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure.BaseClasses;
using TonusClub.UIControls.BaseClasses;

namespace TonusClub.SettingsModule.Views.ContainedControls
{
    public partial class CorporateContracts
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public CorporateContracts()
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
            if (CorpTree != null)
            {
                CorpTree.ExpandAll();
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditCorporateWindow>(() => Model.RefreshCorporates());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CorporatesView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditCorporateWindow>(w => Model.RefreshCorporates(), new ParameterOverride("corporate", ViewModelBase.Clone<Corporate>(Model.CorporatesView.CurrentItem)));
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CorporatesView.CurrentItem == null) return;
            var corp = Model.CorporatesView.CurrentItem as Corporate;
            TonusWindow.Confirm("Удаление", "Удалить корпоративный договор \"" + corp.Name + "\"?\nКлиенты потеряют признак корпоративных.",
            w =>
            {
                if (w.DialogResult ?? false)
                {
                    if (ClientContext.DeleteCorporate(corp.Id))
                    {
                        Model.Corporates.Remove(corp);
                        Model.CorporatesView.Refresh();
                    }
                }
            });
        }

        private void CorporatesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("FranchCorpsMgmt"))
            {
                EditButton_Click(null, null);
            }
        }

        private void NewFolderClick(object sender, RoutedEventArgs e)
        {
            var id = Guid.Empty;
            if (Model.CurrentCorpTreeItem != null && Model.CurrentCorpTreeItem.Id != SettingsLargeViewModel.DeletedFolderId) id = Model.CurrentCorpTreeItem.Id;
            ProcessUserDialog<NewEditCompanyFolderWindow>(
                () => Model.RefreshCompanyFolders(),
                new ParameterOverride("parentId", id), new ParameterOverride("categoryId", 2));
        }

        private void EditFolderClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentCorpTreeItem != null && Model.CurrentCorpTreeItem.Id != Guid.Empty && Model.CurrentCorpTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ProcessUserDialog<NewEditCompanyFolderWindow>(
                    () => Model.RefreshCompanyFolders(),
                    new ParameterOverride("folder", Model.CurrentCorpTreeItem),
                    new ParameterOverride("parentId", Guid.Empty), new ParameterOverride("categoryId", 2));

            }
        }

        private void RemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentCorpTreeItem != null && Model.CurrentCorpTreeItem.Id != Guid.Empty && Model.CurrentCorpTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                TonusWindow.Confirm("Удаление папки", "Удалить папку?\nСодержимое этой папки будут перемещено уровнем выше.", w =>
                {
                    if (w.DialogResult ?? false)
                    {
                        ClientContext.DeleteSettingsFolder(Model.CurrentCorpTreeItem.Id);
                        Model.RefreshCompanyFolders();
                    }
                });
            }
        }

        private void CorpTree_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Model.CurrentCorpTreeItem = (e.Source as Telerik.Windows.Controls.RadTreeView).SelectedItem as CompanySettingsFolder;
        }
    }
}
