using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using TonusClub.ServiceModel;
using System.ComponentModel;
using TonusClub.UIControls;
using Microsoft.Practices.Unity;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    public partial class EmployeePreferencesWindow
    {
        public Employee Employee { get; set; }
        public ICollectionView PreferencesView { get; set; }

        private List<VacationPreference> _preferences;

        IUnityContainer _container;

        public EmployeePreferencesWindow(IUnityContainer container, ClientContext context, Employee employee)
            : base(context)
        {
            _container = container;

            InitializeComponent();
            Employee = employee;

            _preferences = context.GetEmployeePreferences(employee.Id);

            PreferencesView = CollectionViewSource.GetDefaultView(_preferences);

            DataContext = this;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ModuleViewBase.ProcessUserDialog<AddEmployeePreference>(_container, () =>
            {
                _preferences.Clear();
                _preferences.AddRange(_context.GetEmployeePreferences(Employee.Id));
                PreferencesView.Refresh();
            }, new ParameterOverride("employee", Employee));

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (PreferencesView.CurrentItem == null) return;
            _context.DeleteObject("VacationPreferences", ((VacationPreference)PreferencesView.CurrentItem).Id);
            _preferences.Clear();
            _preferences.AddRange(_context.GetEmployeePreferences(Employee.Id));
            PreferencesView.Refresh();
        }
    }
}
