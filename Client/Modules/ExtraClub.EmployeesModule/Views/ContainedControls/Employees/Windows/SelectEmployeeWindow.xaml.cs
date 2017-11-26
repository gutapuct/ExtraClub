using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;
using System.ComponentModel;

namespace ExtraClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    /// <summary>
    /// Interaction logic for SelectEmployeeWindow.xaml
    /// </summary>
    public partial class SelectEmployeeWindow
    {
        public Employee EmployeeResult { get; private set; }
        public ICollectionView Employees { get; set; } 

        public SelectEmployeeWindow(ClientContext context)
        {
            InitializeComponent();
            Employees = CollectionViewSource.GetDefaultView(context.GetEmployees(true, false));
            DataContext = this;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RadGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ModuleViewBase.IsRowClicked(e))
            {
                EmployeeResult = Employees.CurrentItem as Employee;
                DialogResult = true;
                Close();
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            EmployeeResult = Employees.CurrentItem as Employee;
            if (EmployeeResult == null) return;
            DialogResult = true;
            Close();
        }
    }
}
