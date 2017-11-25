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
using TonusClub.UIControls;
using TonusClub.SettingsModule.ViewModels;
using Telerik.Windows.Controls;
using TonusClub.ServiceModel;
using Microsoft.Practices.Unity;
using TonusClub.UIControls.Windows;
using TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for TextActions.xaml
    /// </summary>
    public partial class TextActions : ModuleViewBase
    {

        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public TextActions()
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
            if (FrInformTree != null)
            {
                FrInformTree.ExpandAll();
            }
        }

        private void ActionsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("FrancInfosMgmt"))
            {
                EditActionButton_Click(null, null);
            }
        }

        private void NewActionButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditTextActionWindow>(() => Model.RefreshActions());
        }

        private void EditActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.ActionsView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditTextActionWindow>(
                    () => Model.RefreshActions(),
                    new ResolverOverride[] { new ParameterOverride("action", Model.ActionsView.CurrentItem) });
            }
        }

        private void DeleteActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.ActionsView.CurrentItem == null) return;
            TonusWindow.Confirm("Удаление",
                 "Удалить выделенный информер?",
                e1 =>
                {
                    if ((e1.DialogResult ?? false))
                    {
                        ClientContext.DeleteObject("TextActions", ((TextAction)Model.ActionsView.CurrentItem).Id);
                        Model.RefreshActions();
                    }
                }
            );
        }

        private void NewFolderClick(object sender, RoutedEventArgs e)
        {
            var id = Guid.Empty;
            if (Model.CurrentFrInformTreeItem != null && Model.CurrentFrInformTreeItem.Id != SettingsLargeViewModel.DeletedFolderId) id = Model.CurrentFrInformTreeItem.Id;
            ProcessUserDialog<NewEditCompanyFolderWindow>(() => Model.RefreshCompanyFolders(),
                new ResolverOverride[] { new ParameterOverride("parentId", id), new ParameterOverride("categoryId", 0) });

        }

        private void EditFolderClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentFrInformTreeItem != null && Model.CurrentFrInformTreeItem.Id != Guid.Empty && Model.CurrentFrInformTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ProcessUserDialog<NewEditCompanyFolderWindow>(
                    () => Model.RefreshCompanyFolders(),
                    new ResolverOverride[] { new ParameterOverride("folder", Model.CurrentFrInformTreeItem), new ParameterOverride("parentId", Guid.Empty), new ParameterOverride("categoryId", 0) });
            }
        }

        private void RemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentFrInformTreeItem != null && Model.CurrentFrInformTreeItem.Id != Guid.Empty && Model.CurrentFrInformTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                TonusWindow.Confirm("Удаление папки", "Удалить папку?\nСодержимое этой папки будут перемещено уровнем выше.",w=>
                {
                    if (w.DialogResult ?? false)
                    {
                        ClientContext.DeleteSettingsFolder(Model.CurrentFrInformTreeItem.Id);
                        Model.RefreshCompanyFolders();
                    }
                });
            }
        }

        private void FrInformTree_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Model.CurrentFrInformTreeItem = (e.Source as Telerik.Windows.Controls.RadTreeView).SelectedItem as CompanySettingsFolder;
        }
    }
}
