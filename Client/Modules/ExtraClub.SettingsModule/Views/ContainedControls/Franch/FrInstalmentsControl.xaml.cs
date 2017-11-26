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
using ExtraClub.ServiceModel;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Franch
{
    public partial class FrInstalmentsControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public FrInstalmentsControl()
        {
            InitializeComponent();
        }

        private void EnableInstalmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.FrInstalmentsView.CurrentItem != null)
            {
                ClientContext.PostCompanyInstalmentEnable(((Instalment)Model.FrInstalmentsView.CurrentItem).Id, true);
                ((Instalment)Model.FrInstalmentsView.CurrentItem).Helper = true;
            }
        }

        private void DisableInstalmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.FrInstalmentsView.CurrentItem != null)
            {
                ClientContext.PostCompanyInstalmentEnable(((Instalment)Model.FrInstalmentsView.CurrentItem).Id, false);
                ((Instalment)Model.FrInstalmentsView.CurrentItem).Helper = false;
            }
        }

        private void Tree_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Model.FrCurrentInstalmentTreeItem = (e.Source as Telerik.Windows.Controls.RadTreeView).SelectedItem as SettingsFolder;
        }
    }
}
