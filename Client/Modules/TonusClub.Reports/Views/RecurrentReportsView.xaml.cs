using Microsoft.Practices.Unity;
using System.Windows;
using System.Windows.Input;
using TonusClub.Reports.ViewModels;
using TonusClub.Reports.Views.RecurrentReports;
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.Reports.Views
{
    public partial class RecurrentReportsView : ModuleViewBase
    {
        public RecurrentReportsView(RecurrentReportsViewModel model)
        {
            DataContext = Model = model;
            InitializeComponent();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditRecurrentRule>(() => Model.RefreshDataSync());
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if(Model.RecurrentReportsView.CurrentItem == null) return;
            ProcessUserDialog<NewEditRecurrentRule>(() => Model.RefreshDataAsync(), new ParameterOverride("item", Model.RecurrentReportsView.CurrentItem));
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if(Model.RecurrentReportsView.CurrentItem == null) return;
            ClientContext.DeleteObject("ReportRecurrencies", ((ReportRecurrency)Model.RecurrentReportsView.CurrentItem).Id);
            Model.RefreshDataSync();
        }

        private void RecurrentGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(IsRowClicked(e)) Edit_Click(null, null);
        }

        public RecurrentReportsViewModel Model { get; set; }
    }
}
