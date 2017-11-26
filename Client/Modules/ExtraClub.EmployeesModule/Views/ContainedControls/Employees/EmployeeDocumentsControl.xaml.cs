using System.Windows;
using System.Windows.Input;
using ExtraClub.EmployeesModule.ViewModels;
using ExtraClub.ServiceModel;

namespace ExtraClub.EmployeesModule.Views.ContainedControls.Employees
{
    public partial class EmployeeDocumentsControl
    {
        private EmployeesLargeViewModel Model => DataContext as EmployeesLargeViewModel;

        public EmployeeDocumentsControl()
        {
            InitializeComponent();
        }

        private void EmployeeDocumentsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                var doc = (EmployeeDocument)Model.EmployeeDocumentsView.CurrentItem;
                if (doc.ReportParameter.HasValue)
                {
                    switch (doc.DocType)
                    {
                        case (short)DocumentTypes.CategoryChange:
                            ReportManager.ProcessPdfReport(()=>ClientContext.GenerateEmployeeCategoryOrder(doc.ReportParameter.Value));
                            break;
                        case (short)DocumentTypes.JobApply:
                            ReportManager.ProcessPdfReport(() => ClientContext.GenerateApplyOrder(doc.ReportParameter.Value));
                            break;
                        case (short)DocumentTypes.JobChange:
                            ReportManager.ProcessPdfReport(() => ClientContext.GenerateJobChangeOrder(doc.ReportParameter.Value));
                            break;
                        case (short)DocumentTypes.Trip:
                            ReportManager.ProcessPdfReport(() => ClientContext.GenerateEmployeeTripOrder(doc.ReportParameter.Value));
                            break;
                        case (short)DocumentTypes.Vacation:
                            ReportManager.ProcessPdfReport(() => ClientContext.GenerateEmployeeVacationOrder( doc.ReportParameter.Value));
                            break;
                        case (short)DocumentTypes.JobFire:
                            ReportManager.ProcessPdfReport(() => ClientContext.GenerateEmployeeFireOrder(doc.ReportParameter.Value));
                            break;
                    }
                }
            }
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Model.PrintEmployeeDocuments();
        }
    }
}
