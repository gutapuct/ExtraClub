using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure;
using TonusClub.ServiceModel;

namespace TonusClub.ClientDal.Wizards.NewClub
{
    public partial class TreatmentsControl
    {
        NewDivisionWizard Model
        {
            get
            {
                return (NewDivisionWizard)DataContext;
            }
        }
        public TreatmentsControl()
        {
            InitializeComponent();
        }

        private void NewTreatmentButton_Click(object sender, RoutedEventArgs e)
        {
            var w = ApplicationDispatcher.UnityContainer.Resolve<NewEditTreatmentWindow>(
                new ParameterOverride("context", Model.Context),
                new ParameterOverride("divId", Model.Division.Id));
            w.ShowDialog();
            Model.RefreshTreatments();

        }

        private void EditTreatmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Treatments.CurrentItem != null)
            {
                var w = ApplicationDispatcher.UnityContainer.Resolve<NewEditTreatmentWindow>(
                    new ParameterOverride("treatment", Model.Treatments.CurrentItem),
                    new ParameterOverride("divId", Model.Division.Id),
                    new ParameterOverride("context", Model.Context));
                w.ShowDialog();
                Model.RefreshTreatments();
            }

        }

        private void DeleteTreatmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Treatments.CurrentItem == null) return;
            var tre = (Treatment)Model.Treatments.CurrentItem;
            tre.IsActive = false;
            Model.Context.PostTreatment(tre);
            Model.RefreshTreatments();
        }

        private void TreatmentsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                EditTreatmentButton_Click(null, null);
            }
        }
    }
}
