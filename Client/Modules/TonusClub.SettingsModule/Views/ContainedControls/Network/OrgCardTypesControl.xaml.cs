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
using TonusClub.UIControls;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;
using Telerik.Windows.Controls.GridView;
using TonusClub.UIControls.Windows;
using TonusClub.SettingsModule.Views.ContainedControls.Network.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls
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
        //    TonusWindow.Confirm(new DialogParameters
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
                TonusWindow.Confirm("Удаление папки", "Удалить папку?\nСодержимое этой папки будут перемещено уровнем выше.", w =>
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
