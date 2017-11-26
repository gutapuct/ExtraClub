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
using ExtraClub.TurnoverModule.ViewModels;
using ExtraClub.ServiceModel;
using ExtraClub.TurnoverModule.Views.Windows;
using Microsoft.Practices.Unity;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Windows;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.UIControls.BaseClasses;

//using Telerik.Windows.Controls;

namespace ExtraClub.TurnoverModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for ProvidersControl.xaml
    /// </summary>
    public partial class ProvidersControl : ModuleViewBase
    {
        private TurnoverLargeViewModel Model
        {
            get
            {
                return (TurnoverLargeViewModel)DataContext;
            }
        }

        public ProvidersControl()
        {
            InitializeComponent();
            this.GotFocus += new RoutedEventHandler(ProvidersControl_GotFocus);
        }

        void ProvidersControl_GotFocus(object sender, RoutedEventArgs e)
        {
            Model.UpdateFinished -= new EventHandler(Model_UpdateFinished);
            Model.UpdateFinished += new EventHandler(Model_UpdateFinished);
        }

        void Model_UpdateFinished(object sender, EventArgs e)
        {
            if (ProvidersTree != null)
            {
                ProvidersTree.ExpandAll();
            }
        }

        private void RadTreeView_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Model.CurrentProviderTreeItem = (e.Source as Telerik.Windows.Controls.RadTreeView).SelectedItem as ProviderFolder;
        }

        private void NewProviderFolderClick(object sender, RoutedEventArgs e)
        {
            var id = Guid.Empty;
            if (Model.CurrentProviderTreeItem != null) id = Model.CurrentProviderTreeItem.Id;
            ProcessUserDialog<NewEditProviderFolderWindow>(
                () => Model.RefreshProviderFolders(),
                new ResolverOverride[] { new ParameterOverride("parentId", id) });
        }

        private void EditProviderFolderClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentProviderTreeItem != null && Model.CurrentProviderTreeItem.Id != Guid.Empty)
            {
                ProcessUserDialog<NewEditProviderFolderWindow>(
                    () => Model.RefreshProviderFolders(),
                    new ResolverOverride[] { new ParameterOverride("folder", Model.CurrentProviderTreeItem), new ParameterOverride("parentId", Guid.Empty) });
            }
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentProviderTreeItem != null && Model.CurrentProviderTreeItem.Id != Guid.Empty)
            {
                ExtraWindow.Confirm(
                    UIControls.Localization.Resources.DeleteFolder,
                    UIControls.Localization.Resources.RemoveFolderMessage,
                    e1 =>
                    {
                        if (e1.DialogResult ?? false)
                        {
                            ClientContext.DeleteProviderFolder(Model.CurrentProviderTreeItem.Id);
                            Model.RefreshProviderFolders();
                        }
                    });
            }
        }

        private void ProvidersView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                EditProviderButton_Click(null, null);
            }
        }

        private void EditProviderButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.ProvidersView != null && Model.ProvidersView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditProviderWindow>(
                    () => Model.RefreshProviders(),
                    new ParameterOverride("provider", ViewModelBase.Clone<Provider>(Model.ProvidersView.CurrentItem)), new ParameterOverride("folderId", Guid.Empty));
            }
        }

        private void NewProviderClick(object sender, RoutedEventArgs e)
        {
            var id = Guid.Empty;
            if (Model.CurrentProviderTreeItem != null) id = Model.CurrentProviderTreeItem.Id;

            ProcessUserDialog<NewEditProviderWindow>(
                () => Model.RefreshProviders(),
                new ParameterOverride("folderId", id));
        }

        private void CopyProviderClick(object sender, RoutedEventArgs e)
        {
            if (Model.ProvidersView != null && Model.ProvidersView.CurrentItem != null)
            {
                var provider = ViewModelBase.Clone<Provider>(Model.ProvidersView.CurrentItem);
                provider.Id = Guid.NewGuid();
                provider.Name = UIControls.Localization.Resources.Copy + " " + provider.Name;
                ProcessUserDialog<NewEditProviderWindow>(
                    () => Model.RefreshProviders(),
                    new ParameterOverride("provider", provider),
                    new ParameterOverride("folderId", Guid.Empty));
            }
        }

        private void RemoveProviderClick(object sender, RoutedEventArgs e)
        {
            if (Model.ProvidersView != null && Model.ProvidersView.CurrentItem != null)
            {
                ExtraWindow.Confirm(UIControls.Localization.Resources.HideContragent,
                    UIControls.Localization.Resources.HideContragentMessage,
                    e1 =>
                    {
                        if (e1.DialogResult ?? false)
                        {
                            ClientContext.HideProvider(((Provider)Model.ProvidersView.CurrentItem).Id);
                            Model.RefreshProviders();
                        }
                    });
            }
        }
    }
}
