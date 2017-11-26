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
    public partial class CompanyFinancesControl
    {
        private TurnoverLargeViewModel Model
        {
            get
            {
                return (TurnoverLargeViewModel)DataContext;
            }
        }

        public CompanyFinancesControl()
        {
            InitializeComponent();
        }

        private void AddCompanyFinanceClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditCompanyFinanceWindow>(() => Model.RefreshCompanyFinances());
        }

        private void EditCompanyFinanceClick(object sender, RoutedEventArgs e)
        {
            if (Model.CompanyFinancesView.CurrentItem == null) return;
            ProcessUserDialog<NewEditCompanyFinanceWindow>(() => Model.RefreshCompanyFinances(), new ParameterOverride("fin", ViewModelBase.Clone<CompanyFinance>(Model.CompanyFinancesView.CurrentItem)));
        }

        private void CompFinancesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                EditCompanyFinanceClick(null, null);
            }
        }
    }
}
