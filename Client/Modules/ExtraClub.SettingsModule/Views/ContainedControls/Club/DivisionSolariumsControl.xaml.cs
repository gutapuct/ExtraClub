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
using Microsoft.Practices.Unity;
using ExtraClub.SettingsModule.ViewModels;
using Telerik.Windows.Controls;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.Windows;
using ExtraClub.SettingsModule.Views.ContainedControls.Franch.Windows;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Club
{
    /// <summary>
    /// Interaction logic for DivisionSolariumsControl.xaml
    /// </summary>
    public partial class DivisionSolariumsControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public DivisionSolariumsControl()
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
            if (SolTree != null)
            {
                SolTree.ExpandAll();
            }
        }

        private void NewSolariumButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditSolariumWindow>(() => Model.RefreshSolariums());
        }

        private void EditSolariumButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SolariumsView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditSolariumWindow>(
                    () => Model.RefreshSolariums(),
                    new ParameterOverride("solarium", Model.SolariumsView.CurrentItem));
            }
        }

        private void DeleteSolariumButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SolariumsView.CurrentItem == null) return;
            //ExtraWindow.Confirm(new DialogParameters
            //{
            //    Header = "Удаление",
            //    Content = "Удалить выделенный солярий?",
            //    OkButtonContent = "Да",
            //    CancelButtonContent = "Нет",
            //    Closed = delegate(object sender1, WindowClosedEventArgs e1)
            //    {
            //        if ((e1.DialogResult ?? false))
            //        {
            //            ClientContext.DeleteObject("Solariums", ((Solarium)Model.SolariumsView.CurrentItem).Id);
            //            Model.RefreshSolariums();
            //        }
            //    },
            //    Owner = Application.Current.MainWindow
            //});
            var sol = (Solarium)Model.SolariumsView.CurrentItem;
            sol.IsActive = false;
            ClientContext.PostSolarium(sol);
            Model.RefreshSolariums();
        }

        private void SolariumsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("ClubSolsMgmt"))
            {
                EditSolariumButton_Click(null, null);
            }
        }

        private void NewFolderClick(object sender, RoutedEventArgs e)
        {
            var id = Guid.Empty;
            if (Model.CurrentSolTreeItem != null && Model.CurrentSolTreeItem.Id != SettingsLargeViewModel.DeletedFolderId) id = Model.CurrentSolTreeItem.Id;
            ProcessUserDialog<NewEditCompanyFolderWindow>(() => Model.RefreshCompanyFolders(), new ResolverOverride[] { new ParameterOverride("parentId", id), new ParameterOverride("categoryId", 4) });
        }

        private void EditFolderClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentSolTreeItem != null && Model.CurrentSolTreeItem.Id != Guid.Empty && Model.CurrentSolTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ProcessUserDialog<NewEditCompanyFolderWindow>(() => Model.RefreshCompanyFolders(), new ResolverOverride[] { new ParameterOverride("folder", Model.CurrentSolTreeItem), new ParameterOverride("parentId", Guid.Empty), new ParameterOverride("categoryId", 4) });
            }
        }

        private void RemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentSolTreeItem != null && Model.CurrentSolTreeItem.Id != Guid.Empty && Model.CurrentSolTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ExtraWindow.Confirm("Удаление папки", "Удалить папку?\nСодержимое этой папки будут перемещено уровнем выше.", w =>
                {
                    if (w.DialogResult ?? false)
                    {
                        ClientContext.DeleteSettingsFolder(Model.CurrentSolTreeItem.Id);
                        Model.RefreshCompanyFolders();
                    }
                });
            }
        }

        private void SolTree_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Model.CurrentSolTreeItem = (e.Source as Telerik.Windows.Controls.RadTreeView).SelectedItem as CompanySettingsFolder;
        }
    }
}
