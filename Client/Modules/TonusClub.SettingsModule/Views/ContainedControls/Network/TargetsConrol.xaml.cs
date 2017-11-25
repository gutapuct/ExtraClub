using Microsoft.Practices.Unity;
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
using TonusClub.ServiceModel;
using TonusClub.SettingsModule.ViewModels;
using TonusClub.SettingsModule.Views.ContainedControls.Network.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls.Network
{
    public partial class TargetsConrol
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public TargetsConrol()
        {
            InitializeComponent();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditTargetWindow>(() => Model.RefreshTargets());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.TargetsView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditTargetWindow>(() => Model.RefreshTargets(), new ParameterOverride("target", Model.TargetsView.CurrentItem));
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.TargetsView.CurrentItem != null)
            {
                ClientContext.HideTargetTypeById(((CustomerTargetType)Model.TargetsView.CurrentItem).Id);
                Model.RefreshTargets();
            }
        }

        private void TargetsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                EditButton_Click(null, null);
            }
        }
    }
}
