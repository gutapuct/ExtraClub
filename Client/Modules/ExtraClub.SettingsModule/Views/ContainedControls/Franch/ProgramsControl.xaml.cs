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
using ExtraClub.UIControls;
using ExtraClub.SettingsModule.ViewModels;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.Windows;
using ExtraClub.SettingsModule.Views.ContainedControls.Franch.Windows;

namespace ExtraClub.SettingsModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for ProgramsControl.xaml
    /// </summary>
    public partial class ProgramsControl : ModuleViewBase
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public ProgramsControl()
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
            if (ProgramTree != null)
            {
                ProgramTree.ExpandAll();
            }
        }

        private void NewTreatmentProgramButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditTreatmentProgramWindow>(() => Model.RefreshTreatmentPrograms());
        }

        private void EditTreatmentProgramButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.TreatmentProgramsView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditTreatmentProgramWindow>(
                    x => Model.RefreshTreatmentPrograms(),
                    new ParameterOverride("program", Model.TreatmentProgramsView.CurrentItem));
            }
        }

        private void DeleteTreatmentProgramButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.TreatmentProgramsView.CurrentItem == null) return;
            ExtraWindow.Confirm("Удаление",
                 "Удалить программу \"" + ((TreatmentProgram)Model.TreatmentProgramsView.CurrentItem).ProgramName + "\"?",
                e1 =>
                {
                    if ((e1.DialogResult ?? false))
                    {
                        ClientContext.DeleteTreatmentProgram(((TreatmentProgram)Model.TreatmentProgramsView.CurrentItem).Id);
                        Model.RefreshTreatmentPrograms();
                    }
                }
            );
        }

        private void OrgTreatmentPrograms_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("FranchProgramsMgmt"))
            {
                EditTreatmentProgramButton_Click(null, null);
            }
        }

        private void NewFolderClick(object sender, RoutedEventArgs e)
        {
            var id = Guid.Empty;
            if (Model.CurrentProgramTreeItem != null && Model.CurrentProgramTreeItem.Id != SettingsLargeViewModel.DeletedFolderId) id = Model.CurrentProgramTreeItem.Id;
            ProcessUserDialog<NewEditCompanyFolderWindow>(() => Model.RefreshCompanyFolders(),
                new ResolverOverride[] { new ParameterOverride("parentId", id), new ParameterOverride("categoryId", 1) });
        }

        private void EditFolderClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentProgramTreeItem != null && Model.CurrentProgramTreeItem.Id != Guid.Empty && Model.CurrentProgramTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ProcessUserDialog<NewEditCompanyFolderWindow>(() => Model.RefreshCompanyFolders(),
                        new ParameterOverride("folder", Model.CurrentProgramTreeItem),
                        new ParameterOverride("parentId", Guid.Empty), new ParameterOverride("categoryId", 1));
            }
        }

        private void RemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentProgramTreeItem != null && Model.CurrentProgramTreeItem.Id != Guid.Empty && Model.CurrentProgramTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ExtraWindow.Confirm("Удаление папки", "Удалить папку?\nСодержимое этой папки будут перемещено уровнем выше.",
                w =>
                {
                    if (w.DialogResult ?? false)
                    {
                        ClientContext.DeleteSettingsFolder(Model.CurrentProgramTreeItem.Id);
                        Model.RefreshCompanyFolders();
                    }
                });
            }
        }

        private void ProgramTree_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Model.CurrentProgramTreeItem = (e.Source as Telerik.Windows.Controls.RadTreeView).SelectedItem as CompanySettingsFolder;
        }
    }
}
