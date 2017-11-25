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
    public partial class TargetConfigsControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public TargetConfigsControl()
        {
            InitializeComponent();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditTrgetTypeSetWindow>(Model.RefreshTargetConfigs);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if(Model.TargetConfigsView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditTrgetTypeSetWindow>(Model.RefreshTargetConfigs, new ParameterOverride("set", Model.TargetConfigsView.CurrentItem));
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if(Model.TargetConfigsView.CurrentItem != null)
            {
                ClientContext.DeleteObject("TargetTypeSets", ((TargetTypeSet)Model.TargetConfigsView.CurrentItem).Id);
                Model.RefreshTargetConfigs();
            }
        }

        private void TargetConfigsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(IsRowClicked(e))
            {
                EditButton_Click(null, null);
            }
        }
    }
}
