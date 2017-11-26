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
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.Windows;
using ExtraClub.SettingsModule.Views.ContainedControls.Network.Windows;

namespace ExtraClub.SettingsModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for OrgTreatmentTypesControl.xaml
    /// </summary>
    public partial class OrgTreatmentTypesControl
    {
        public OrgTreatmentTypesControl()
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
            if (TreatmentsTree != null)
            {
                TreatmentsTree.ExpandAll();
            }
        }


        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        private void NewTreatmentTypeButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditTreatmentTypeWindow>(() => Model.RefreshOrgTreatmentTypes());
        }

        private void EditTreatmentTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.OrgTreatmentTypesView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditTreatmentTypeWindow>(() => Model.RefreshOrgTreatmentTypes(), new ParameterOverride("treatmentType", Model.OrgTreatmentTypesView.CurrentItem));
            }
        }

        private void EnableTreatmentTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.OrgTreatmentTypesView.CurrentItem == null) return;
            ClientContext.SetObjectActive("TreatmentTypes", ((TreatmentType)Model.OrgTreatmentTypesView.CurrentItem).Id, true);
            Model.RefreshOrgTreatmentTypes();
        }

        private void DisableTreatmentTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.OrgTreatmentTypesView.CurrentItem == null) return;
            ClientContext.SetObjectActive("TreatmentTypes", ((TreatmentType)Model.OrgTreatmentTypesView.CurrentItem).Id, false);
            Model.RefreshOrgTreatmentTypes();
        }

        private void OrgTreatmentTypes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("NetTreatsMgmt"))
            {
                EditTreatmentTypeButton_Click(null, null);
            }
        }

        private void NewTreatmentConfigButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditTreatmentConfigWindow>(() => Model.RefreshOrgTreatmentConfigs());
        }

        private void EditTreatmentConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.OrgTreatmentConfigsView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditTreatmentConfigWindow>(() => Model.RefreshOrgTreatmentConfigs(), new ParameterOverride("treatmentConfig", Model.OrgTreatmentConfigsView.CurrentItem));
            }
        }
        
        private void EnableTreatmentConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.OrgTreatmentConfigsView.CurrentItem == null) return;
            ClientContext.SetObjectActive("TreatmentConfigs", ((TreatmentConfig)Model.OrgTreatmentConfigsView.CurrentItem).Id, true);
            Model.RefreshOrgTreatmentConfigs();
        }

        private void DisableTreatmentConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.OrgTreatmentConfigsView.CurrentItem == null) return;
            ClientContext.SetObjectActive("TreatmentConfigs", ((TreatmentConfig)Model.OrgTreatmentConfigsView.CurrentItem).Id, false);
            Model.RefreshOrgTreatmentConfigs();
        }

        private void OrgTreatmentConfigs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("NetTreatsMgmt"))
            {
                EditTreatmentConfigButton_Click(null, null);
            }
        }


        private void NewFolderClick(object sender, RoutedEventArgs e)
        {
            var id = Guid.Empty;
            if (Model.CurrentTreatmentConfigTreeItem != null && Model.CurrentTreatmentConfigTreeItem.Id != SettingsLargeViewModel.DeletedFolderId) id = Model.CurrentTreatmentConfigTreeItem.Id;
            ProcessUserDialog<NewEditFolderWindow>(
                () => Model.RefreshFolders(),
                new ResolverOverride[] { new ParameterOverride("parentId", id), new ParameterOverride("categoryId", 3) });
            ;
        }

        private void EditFolderClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentTreatmentConfigTreeItem != null && Model.CurrentTreatmentConfigTreeItem.Id != Guid.Empty && Model.CurrentTreatmentConfigTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ProcessUserDialog<NewEditFolderWindow>(
                    () => Model.RefreshFolders(),
                    new ResolverOverride[] { new ParameterOverride("folder", Model.CurrentTreatmentConfigTreeItem), new ParameterOverride("parentId", Guid.Empty), new ParameterOverride("categoryId", 3) });

            }
        }

        private void RemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentTreatmentConfigTreeItem != null && Model.CurrentTreatmentConfigTreeItem.Id != Guid.Empty && Model.CurrentTreatmentConfigTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ExtraWindow.Confirm("Удаление папки", "Удалить папку?\nСодержимое этой папки будут перемещено уровнем выше.",
                w =>
                {
                    if (w.DialogResult ?? false)
                    {
                        ClientContext.DeleteSettingsFolder(Model.CurrentTreatmentConfigTreeItem.Id);
                        Model.RefreshFolders();
                        Model.RefreshOrgTreatmentConfigs();
                    }
                });
            }
        }

        private void TicketTree_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Model.CurrentTreatmentConfigTreeItem = (e.Source as Telerik.Windows.Controls.RadTreeView).SelectedItem as SettingsFolder;
        }

    }
}
