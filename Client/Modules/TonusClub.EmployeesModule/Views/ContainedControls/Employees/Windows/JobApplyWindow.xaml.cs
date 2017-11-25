using System;
using System.Windows;
using TonusClub.ServiceModel;
using TonusClub.EmployeesModule.ViewModels;
using TonusClub.UIControls;
using TonusClub.UIControls.Interfaces;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    public partial class JobApplyWindow
    {
        private readonly ApplyChangeJobViewModel _model;

        private readonly IReportManager _repMan;

        public JobApplyWindow(ClientContext context, Employee employee, IReportManager repMan)
        {
            _model = new ApplyChangeJobViewModel(employee, context);

            _repMan = repMan;

            InitializeComponent();
            DataContext = _model;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(_model.JobPlacement.Error)) return;
            if (_model.JobPlacement.ApplyDate < DateTime.Today.AddYears(-10)) return;
            _context.PostJobPlacement(_model.JobPlacement);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.AssetApply())
            {
                if (PrintApplyOrder.IsChecked ?? false)
                {
                    _repMan.ProcessPdfReport(() => _context.GenerateApplyOrder(_model.JobPlacement.Id));
                }
                if (PrintJobAgreement.IsChecked ?? false)
                {
                    _repMan.ProcessPdfReport(() => _context.GenerateJobAgreement(_model.Employee.Id));
                }
                if (PrintResponsibleAgreement.IsChecked ?? false)
                {
                    _repMan.ProcessPdfReport(() => _context.GenerateResponsibleAgreement(_model.Employee.Id));
                }
                if (PrintSecretAgreement.IsChecked ?? false)
                {
                    _repMan.ProcessPdfReport(() => _context.GenerateSecretAgreement(_model.Employee.Id));
                }
                if (PrintJobDescription.IsChecked ?? false)
                {
                    _repMan.ProcessPdfReport(() => _context.GenerateJobDescription(_model.Employee.Id));
                }
                if (EmitCardBox.IsChecked ?? false)
                {
                    var d = new EmitCardWindow(_model.Employee);
                    d.ShowDialog();
                }
                DialogResult = true;
                Close();
            }
        }
    }
}
