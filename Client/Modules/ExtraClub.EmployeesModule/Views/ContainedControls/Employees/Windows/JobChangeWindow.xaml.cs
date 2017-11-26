using System.Windows;
using ExtraClub.ServiceModel;
using ExtraClub.EmployeesModule.ViewModels;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Interfaces;

namespace ExtraClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    /// <summary>
    /// Interaction logic for JobChangeWindow.xaml
    /// </summary>
    public partial class JobChangeWindow
    {
        readonly ApplyChangeJobViewModel _model;
        private readonly IReportManager _repMan;

        public JobChangeWindow(ClientContext context, Employee employee, IReportManager repMan)
        {
            _model = new ApplyChangeJobViewModel(employee, context);
            _model.RemoveJob(employee.SerializedJobPlacement.JobId);
            _repMan = repMan;

            InitializeComponent();
            DataContext = _model;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.AssetChange())
            {
                if (PrintJobChangeOrder.IsChecked ?? false)
                {
                    _repMan.ProcessPdfReport(() => _context.GenerateJobChangeOrder(_model.JobPlacement.Id));
                }
                DialogResult = true;
                Close();
            }
        }
    }
}