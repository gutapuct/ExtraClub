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
using TonusClub.SettingsModule.ViewModels;
using TonusClub.ServiceModel;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch
{
    /// <summary>
    /// Interaction logic for CompanyCardsSettingsControl.xaml
    /// </summary>
    public partial class CompanyCardsSettingsControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public CompanyCardsSettingsControl()
        {
            InitializeComponent();
        }

        private void EnableCardTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.FrCardTypesView.CurrentItem != null)
            {
                ClientContext.PostCompanyCardTypeEnable(((CustomerCardType)Model.FrCardTypesView.CurrentItem).Id, true);
                ((CustomerCardType)Model.FrCardTypesView.CurrentItem).Helper = true;
            }
        }

        private void DisableCardTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.FrCardTypesView.CurrentItem != null)
            {
                ClientContext.PostCompanyCardTypeEnable(((CustomerCardType)Model.FrCardTypesView.CurrentItem).Id, false);
                ((CustomerCardType)Model.FrCardTypesView.CurrentItem).Helper = false;
            }
        }

        private void CardTypes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && Model.FrCardTypesView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditCardTypeWindow>(()=>{}, new ResolverOverride[] { new ParameterOverride("cardType", Model.FrCardTypesView.CurrentItem), new ParameterOverride("readOnly", true) });
            }
        }

        private void CardsTree_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Model.FrCurrentCardTypeTreeItem = (e.Source as Telerik.Windows.Controls.RadTreeView).SelectedItem as SettingsFolder;
        }
    }
}
