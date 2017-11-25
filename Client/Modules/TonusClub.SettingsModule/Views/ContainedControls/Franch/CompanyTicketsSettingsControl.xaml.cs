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
using TonusClub.ServiceModel;
using Microsoft.Practices.Unity;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch
{
    /// <summary>
    /// Interaction logic for CompanyTicketsSettingsControl.xaml
    /// </summary>
    public partial class CompanyTicketsSettingsControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public CompanyTicketsSettingsControl()
        {
            InitializeComponent();
        }

        private void TicketTypes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && Model.FrTicketTypesView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditTicketTypeWindow>(() => { }, new ResolverOverride[] { new ParameterOverride("ticketType", Model.FrTicketTypesView.CurrentItem), new ParameterOverride("readOnly", true) });
            }
        }

        private void EnableTicketTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.FrTicketTypesView.CurrentItem != null)
            {
                ClientContext.PostCompanyTicketTypeEnable(((TicketType)Model.FrTicketTypesView.CurrentItem).Id, true);
                ((TicketType)Model.FrTicketTypesView.CurrentItem).Helper = true;
            }
        }

        private void DisableTicketTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.FrTicketTypesView.CurrentItem != null)
            {
                ClientContext.PostCompanyTicketTypeEnable(((TicketType)Model.FrTicketTypesView.CurrentItem).Id, false);
                ((TicketType)Model.FrTicketTypesView.CurrentItem).Helper = false;
            }
        }

        private void TicketTree_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Model.FrCurrentTicketTypeTreeItem = (e.Source as Telerik.Windows.Controls.RadTreeView).SelectedItem as SettingsFolder;
        }
    }
}
