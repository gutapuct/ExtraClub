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
using ExtraClub.TurnoverModule.Views.ContainedControls.Windows;
using Microsoft.Practices.Unity;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.TurnoverModule.Views.ContainedControls
{
    public partial class FinancesClubControl
    {
        private TurnoverLargeViewModel Model
        {
            get
            {
                return (TurnoverLargeViewModel)DataContext;
            }
        }

        public FinancesClubControl()
        {
            InitializeComponent();
        }

        private void AddDivFinanceClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditDivisionFinanceWindow>(() => Model.RefreshDivisionFinances());
        }

        private void EditDivFinanceClick(object sender, RoutedEventArgs e)
        {
            if (Model.DivisionFinancesView.CurrentItem == null) return;
            ProcessUserDialog<NewEditDivisionFinanceWindow>(() => Model.RefreshDivisionFinances(), new ParameterOverride("fin", ViewModelBase.Clone<DivisionFinance>(Model.DivisionFinancesView.CurrentItem)));
        }

        private void DivFinancesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                EditDivFinanceClick(null, null);
            }
        }
    }
}
