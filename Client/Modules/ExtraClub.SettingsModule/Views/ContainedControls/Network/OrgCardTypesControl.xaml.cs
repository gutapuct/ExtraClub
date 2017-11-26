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
    /// Interaction logic for OrgCardTypesControl.xaml
    /// </summary>
    public partial class OrgCardTypesControl : ModuleViewBase
    {
        public OrgCardTypesControl()
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
            if (CardsTree != null)
            {
                CardsTree.ExpandAll();
            }
        }

        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        private void OrgCardTypes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
            if (originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                if (row != null && ClientContext.CheckPermission("NetCardsMgmt"))
                {
                    EditCardTypeButton_Click(null, null);
                }
            }
        }

        private void NewCardTypeButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditCardTypeWindow>(
                () => Model.RefreshOrgCardTypes(),
                new ResolverOverride[] { new ParameterOverride("readOnly", false) });
        }

        private void EditCardTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedOrgCardType != null)
            {
                ProcessUserDialog<NewEditCardTypeWindow>(w => Model.RefreshOrgCardTypes(), new ResolverOverride[] { new ParameterOverride("cardType", Model.SelectedOrgCardType), new ParameterOverride("readOnly", false) });
            }
        }

        //private void DeleteCardTypeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Model.SelectedOrgCardType == null) return;
        //    if (Model.SelectedOrgCardType.Id == Guid.Empty) return;
        //    ExtraWindow.Confirm(new DialogParameters
        //    {
        //        Header = "Удаление",
        //        Content = "Удалить тип карт \""+Model.SelectedOrgCardType.Name+"\"?",
        //        OkButtonContent = "Да",
        //        CancelButtonContent = "Нет",
        //        Closed = delegate(object sender1, WindowClosedEventArgs e1)
        //        {
        //            if ((e1.DialogResult ?? false))
        //            {
        //                ClientContext.DeleteObject("CustomerCardTypes", Model.SelectedOrgCardType.Id);
        //                Model.RefreshOrgCardTypes();
        //            }
        //        },
        //        Owner = Application.Current.MainWindow
        //    });
        //}

        private void EnableCardTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedOrgCardType == null) return;
            ClientContext.SetObjectActive("CustomerCardTypes", Model.SelectedOrgCardType.Id, true);
            Model.RefreshOrgCardTypes();
        }

        private void DisableCardTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedOrgCardType == null) return;
            ClientContext.SetObjectActive("CustomerCardTypes", Model.SelectedOrgCardType.Id, false);
            Model.RefreshOrgCardTypes();
        }

        private void CardsTree_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Model.CurrentCardTypeTreeItem = (e.Source as Telerik.Windows.Controls.RadTreeView).SelectedItem as SettingsFolder;
        }

        private void NewFolderClick(object sender, RoutedEventArgs e)
        {
            var id = Guid.Empty;
            if (Model.CurrentCardTypeTreeItem != null && Model.CurrentCardTypeTreeItem.Id != SettingsLargeViewModel.DeletedFolderId) id = Model.CurrentCardTypeTreeItem.Id;
            ProcessUserDialog<NewEditFolderWindow>(() =>
            {
                Model.RefreshFolders();
                Model.RefreshCompanyFolders();
            },
                new ResolverOverride[] { new ParameterOverride("parentId", id), new ParameterOverride("categoryId", 0) });

        }

        private void EditFolderClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentCardTypeTreeItem != null && Model.CurrentCardTypeTreeItem.Id != Guid.Empty && Model.CurrentCardTypeTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ProcessUserDialog<NewEditFolderWindow>(() =>
                {
                    Model.RefreshFolders();
                    Model.RefreshCompanyFolders();
                }, new ResolverOverride[] { new ParameterOverride("folder", Model.CurrentCardTypeTreeItem), new ParameterOverride("parentId", Guid.Empty), new ParameterOverride("categoryId", 0) });

            }
        }

        private void RemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentCardTypeTreeItem != null && Model.CurrentCardTypeTreeItem.Id != Guid.Empty && Model.CurrentCardTypeTreeItem.Id != SettingsLargeViewModel.DeletedFolderId)
            {
                ExtraWindow.Confirm("Удаление папки", "Удалить папку?\nСодержимое этой папки будут перемещено уровнем выше.", w =>
                {
                    if (w.DialogResult ?? false)
                    {
                        ClientContext.DeleteSettingsFolder(Model.CurrentCardTypeTreeItem.Id);
                        Model.RefreshFolders();
                    }
                });
            }
        }
    }
}
