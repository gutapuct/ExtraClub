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
using ExtraClub.UIControls;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using Telerik.Windows.Controls.GridView;
using ExtraClub.UIControls.Windows;
using ExtraClub.SettingsModule.Views.ContainedControls.Network.Windows;

namespace ExtraClub.SettingsModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for OrgTicketTypesControl.xaml
    /// </summary>
    public partial class OrgTicketTypesControl : ModuleViewBase
    {
        public OrgTicketTypesControl()
        {
            InitializeComponent();
            this.GotFocus += new RoutedEventHandler(OrgTicketTypesControl_GotFocus);
        }

        void OrgTicketTypesControl_GotFocus(object sender, RoutedEventArgs e)
        {
            Model.UpdateFinished -= new EventHandler(Model_UpdateFinished);
            Model.UpdateFinished += new EventHandler(Model_UpdateFinished);
        }

        void Model_UpdateFinished(object sender, EventArgs e)
        {
            if (TicketsTree != null)
            {
                TicketsTree.ExpandAll();
            }
        }

        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }


        private void OrgTicketTypes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("NetTicketsMgmt"))
            {
                EditTicketTypeButton_Click(null, null);
            }
        }

        private void NewTicketTypeButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditTicketTypeWindow>(() => Model.RefreshOrgTicketTypes(), new ResolverOverride[] { new ParameterOverride("readOnly", false) });
        }

        private void EditTicketTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedOrgTicketType != null)
            {
                ProcessUserDialog<NewEditTicketTypeWindow>(
                    w => Model.RefreshOrgTicketTypes(),
                    new ResolverOverride[] { new ParameterOverride("ticketType", Model.SelectedOrgTicketType), new ParameterOverride("readOnly", false) });
            }
        }

        private void EnableTicketTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedOrgTicketType == null) return;
            ClientContext.SetObjectActive("TicketTypes", Model.SelectedOrgTicketType.Id, true);
            Model.RefreshOrgTicketTypes();
        }

        private void DisableTicketTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedOrgTicketType == null) return;
            ClientContext.SetObjectActive("TicketTypes", Model.SelectedOrgTicketType.Id, false);
            Model.RefreshOrgTicketTypes();
        }

        private void NewFolderClick(object sender, RoutedEventArgs e)
        {
            var id = Guid.Empty;
            if (Model.CurrentTicketTypeTreeItem != null && Model.CurrentTicketTypeTreeItem.Id != SettingsLargeViewModel.DeletedFolderId) id = Model.CurrentTicketTypeTreeItem.Id;
            ProcessUserDialog<NewEditFolderWindow>(
                () => Model.RefreshFolders(),
                new ResolverOverride[] { new ParameterOverride("parentId", id), new ParameterOverride("categoryId", 1) });
            ;
        }

        private void EditFolderClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentTicketTypeTreeItem != null && Model.CurrentTicketTypeTreeItem.Id != Guid.Empty && Model.CurrentTicketTypeTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ProcessUserDialog<NewEditFolderWindow>(
                    () => Model.RefreshFolders(),
                    new ResolverOverride[] { new ParameterOverride("folder", Model.CurrentTicketTypeTreeItem), new ParameterOverride("parentId", Guid.Empty), new ParameterOverride("categoryId", 1) });

            }
        }

        private void RemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentTicketTypeTreeItem != null && Model.CurrentTicketTypeTreeItem.Id != Guid.Empty && Model.CurrentTicketTypeTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ExtraWindow.Confirm("Удаление папки", "Удалить папку?\nСодержимое этой папки будут перемещено уровнем выше.",
                w =>
                {
                    if (w.DialogResult ?? false)
                    {
                        ClientContext.DeleteSettingsFolder(Model.CurrentTicketTypeTreeItem.Id);
                        Model.RefreshFolders();
                    }
                });
            }
        }

        private void TicketTree_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Model.CurrentTicketTypeTreeItem = (e.Source as Telerik.Windows.Controls.RadTreeView).SelectedItem as SettingsFolder;
        }
    }
}
