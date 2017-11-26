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
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.Clients.ViewModels;
using ExtraClub.Infrastructure.ParameterClasses;
using ExtraClub.UIControls;

namespace ExtraClub.Clients.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for ExtraTreatmentsControl.xaml
    /// </summary>
    public partial class ExtraTreatmentsControl
    {
        private ClientLargeViewModel Model
        {
            get
            {
                return DataContext as ClientLargeViewModel;
            }
        }

        public ExtraTreatmentsControl()
        {
            InitializeComponent();
            //this.DataContextChanged += new DependencyPropertyChangedEventHandler(ExtraTreatmentsControl_DataContextChanged);
        }

        void ExtraTreatmentsControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Model != null)
            {
                if (Model.ClientContext.CurrentDivision != null && !Model.ClientContext.CurrentDivision.HasChildren)
                {
                    MainTab.Items.Remove(ChildrenRoomTab);
                }
            }
        }

        private void NewChildButtonClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentCustomer == null) return;
            if (!Model.CurrentCustomer.IsInClub) return;
            NavigationManager.MakeNewChildRequest(Model.CurrentCustomer.Id);
            Model.RefreshChildren();
        }

        private void ChildrenViewGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void NewSolariumPlanButtonClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentCustomer != null)
            {
                NavigationManager.MakeNewSolariumVisitRequest(new NewSolariumVisitParams { CustomerId = Model.CurrentCustomer.Id, CloseAction = () => Model.RefreshSolarium() });
            }
        }

        private void OrganizerViewButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationManager.MakeActivateSolariumScheduleRequest();
        }
    }
}
