using System;
using System.Windows;
using System.Windows.Data;
using ExtraClub.ServiceModel;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    public partial class NewEditEmployeeWindow
    {
        public Employee Employee { get; set; }
        public Customer Customer { get; set; }

        public ICollectionView Cities { get; set; }
        public ICollectionView Streets { get; set; }

        IUnityContainer _container;

        public NewEditEmployeeWindow(ClientContext context, Employee employee, IUnityContainer container)
            : base(context)
        {
            InitializeComponent();

            _container = container;
            if (employee == null || employee.Id == Guid.Empty)
            {
                Employee = new Employee
                {
                    CompanyId = context.CurrentCompany.CompanyId,
                    MainDivisionId = context.CurrentDivision.Id,
                    IsActive = true
                };
                Customer = new Customer
                {
                    CompanyId = context.CurrentCompany.CompanyId
                };
            }
            else
            {
                addJob.Visibility = System.Windows.Visibility.Collapsed;
                Employee = employee;
                Customer = Employee.SerializedCustomer ?? context.GetCustomer(employee.BoundCustomerId);
            }

            var tmp = _context.GetAddressLists();

            Streets = CollectionViewSource.GetDefaultView(tmp[2]);
            Cities = CollectionViewSource.GetDefaultView(tmp[0]);


            this.DataContext = this;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            var id = _context.PostEmployeeForm(Employee, Customer);
            DialogResult = true;
            if (addJob.IsChecked ?? false)
            {
                var d = _container.Resolve<JobApplyWindow>(new ResolverOverride[] { new ParameterOverride("employee", _context.GetEmployeeById(id)) });
                d.ShowDialog();
            }
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            if (Employee.Id == Guid.Empty)
            {
                ExtraWindow.Confirm("Отмена",
                    "Вы отменяете заведение нового сотрудника.\nВведённые данные не будут сохранены.\nПродолжить?",
                    e1 =>
                    {
                        if (e1.DialogResult ?? false)
                        {
                            Close();
                        }
                    });
            }
            else
            {
                Close();
            }
        }
    }
}
