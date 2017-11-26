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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using ExtraClub.ServiceModel;
using ExtraClub.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using ExtraClub.OrganizerModule.Views.Tasks.Windows.NewCallMaster;
using ExtraClub.UIControls.Windows;
using ExtraClub.UIControls;

namespace ExtraClub.OrganizerModule.Views.Tasks.Windows
{
    public partial class NewCallsTask
    {
        public ObservableCollection<Customer> Customers { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }
        IUnityContainer _cont;

        public DateTime RunDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Comments { get; set; }

        List<Employee> employees;

        public NewCallsTask(IUnityContainer container, IEnumerable<Guid> customers = null)
        {
            _cont = container;
            Customers = new ObservableCollection<Customer>();
            Employees = new ObservableCollection<Employee>();

            InitializeComponent();
            RunDate = DateTime.Now;
            ExpiryDate = DateTime.Now.AddDays(1);
            DataContext = this;
            employees = _context.GetEmployees(true, true);
            if (customers != null)
            {
                foreach (var customerId in customers)
                {
                    var cust = _context.GetCustomer(customerId);
                    if (cust != null)
                    {
                        Customers.Add(cust);
                    }
                }
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Comments))
            {
                ExtraWindow.Alert("Ошибка", "Укажите цель звонка!");
                return;
            }
            if (RunDate > ExpiryDate)
            {
                ExtraWindow.Alert("Ошибка", "Дата постановки не может быть после даты исполнения!");
                return;
            }
            _context.PostGroupCall(Customers.Select(i => i.Id).ToArray(), Employees.Select(i => i.Id).ToArray(), Comments, RunDate, ExpiryDate);
            DialogResult = true;
            Close();
        }

        private void AddOneClientClick(object sender, RoutedEventArgs e)
        {
            var wnd = _cont.Resolve<SelectOneCustomerWindow>();
            wnd.ClosedParams = w =>
            {
                if (w.DialogResult ?? false)
                {
                    if (!Customers.Any(i => i.Id == ((SelectOneCustomerWindow)w).CustomerResult.Id))
                    {
                        Customers.Add(((SelectOneCustomerWindow)w).CustomerResult);
                    }
                }
            };
            wnd.ShowDialog();
        }

        private void RemoveCustomerClick(object sender, RoutedEventArgs e)
        {
            var c = (sender as FrameworkElement).DataContext as Customer;
            if (c == null) return;
            Customers.Remove(c);
        }

        private void AddAllCustomersClick(object sender, RoutedEventArgs e)
        {
            DropdownButton.IsOpen = false;
            Customers.Clear();
            try
            {
                _context.GetAllCustomers().ForEach(i => Customers.Add(i));
            }
            catch
            {
                ExtraWindow.Alert("Ошибка", "При добавлении произошла ошибка.\nПопробуйте добавить клиентов по более избирательному критерию.");
            }
        }

        private void AddPotentialCustomersClick(object sender, RoutedEventArgs e)
        {
            DropdownButton.IsOpen = false;
            try
            {
                _context.GetPotentialCustomers().ForEach(i =>
                {
                    if (!Customers.Any(j => j.Id == i.Id)) Customers.Add(i);
                });
            }
            catch
            {
                ExtraWindow.Alert("Ошибка", "При добавлении произошла ошибка.\nПопробуйте добавить клиентов по более избирательному критерию.");
            }

        }

        private void AddInactiveCustomersClick(object sender, RoutedEventArgs e)
        {
            DropdownButton.IsOpen = false;

            try
            {
                _context.GetInactiveCustomers().ForEach(i =>
                {
                    if (!Customers.Any(j => j.Id == i.Id)) Customers.Add(i);
                });
            }
            catch
            {
                ExtraWindow.Alert("Ошибка", "При добавлении произошла ошибка.\nПопробуйте добавить клиентов по более избирательному критерию.");
            }

        }

        private void AddActiveCustomersClick(object sender, RoutedEventArgs e)
        {
            DropdownButton.IsOpen = false;

            try
            {
                _context.GetActiveCustomers().ForEach(i =>
                {
                    if (!Customers.Any(j => j.Id == i.Id)) Customers.Add(i);
                });
            }
            catch
            {
                ExtraWindow.Alert("Ошибка", "При добавлении произошла ошибка.\nПопробуйте добавить клиентов по более избирательному критерию.");
            }

        }

        private void AddStatusClick(object sender, RoutedEventArgs e)
        {
            var wnd = _cont.Resolve<SelectCustomerStatusesWindow>();
            wnd.AppendCollection<DictionaryPair>(_cont.Resolve<IDictionaryManager>().GetViewSource("CustomerStatuses").SourceCollection as IEnumerable<DictionaryPair>, i => i.Key, i => i.Value);
            wnd.Closed = () =>
            {
                if (wnd.DialogResult ?? false)
                {
                    _context.GetCustomersByStatus(wnd.Result).ForEach(i =>
                    {
                        if (!Customers.Any(j => j.Id == i.Id)) Customers.Add(i);
                    });
                }
            };
            wnd.ShowDialog();
        }

        private void AddManagerClick(object sender, RoutedEventArgs e)
        {
            var wnd = _cont.Resolve<SelectCustomerManagerWindow>();
            wnd.AppendCollection<DictionaryPair>(_cont.Resolve<IDictionaryManager>().GetViewSource("Users").SourceCollection as IEnumerable<DictionaryPair>, i => i.Key, i => i.Value);
            wnd.Closed = () =>
            {
                if (wnd.DialogResult ?? false)
                {
                    _context.GetCustomersByManagers(wnd.Result).ForEach(i =>
                    {
                        if (!Customers.Any(j => j.Id == i.Id)) Customers.Add(i);
                    });
                }
            };
            wnd.ShowDialog();
        }

        private void RemoveEmployeeClick(object sender, RoutedEventArgs e)
        {
            var empl = (sender as FrameworkElement).DataContext as Employee;
            if (empl == null) return;
            Employees.Remove(empl);
        }

        private void AddAllEmployeesClick(object sender, RoutedEventArgs e)
        {
            EmployeeDropdownButton.IsOpen = false;
            try
            {
                employees.ForEach(i =>
                {
                    if (!Employees.Any(j => j.Id == i.Id)) Employees.Add(i);
                });
            }
            catch
            {
                ExtraWindow.Alert("Ошибка", "При добавлении произошла ошибка.\nПопробуйте добавить сотрудников по более избирательному критерию.");
            }
        }

        private void AddOneEmployeeClick(object sender, RoutedEventArgs e)
        {
            NavigationManager.MakeQueryOneEmployee(employee =>
            {
                if (employee != null && !Employees.Any(i => i.Id == employee.Id))
                {
                    Employees.Add(employee);
                }
            });
        }

        private void AddInJobEmployeesClick(object sender, RoutedEventArgs e)
        {
            var wnd = _cont.Resolve<SelectCustomerStatusesWindow>();
            wnd.AppendCollection<Job>(_context.GetJobs(), i => i.Id, i => i.Name);
            wnd.Closed = new Action(() =>
            {
                employees.Where(i => wnd.Result.Contains(i.SerializedJobPlacement.JobId)).ToList().ForEach(i =>
                {
                    if (!Employees.Any(j => j.Id == i.Id)) Employees.Add(i);
                });
            });
            wnd.ShowDialog();
        }

        private void AddInUnitEmployeesClick(object sender, RoutedEventArgs e)
        {
            var wnd = _cont.Resolve<SelectCustomerStatusesWindow>();

            var units = employees.Select(i => i.SerializedJobPlacement.SerializedUnit).Distinct().ToDictionary(i => Guid.NewGuid(), i => i);

            wnd.AppendCollection<KeyValuePair<Guid, string>>(units, i => i.Key, i => i.Value);
            wnd.Closed = () =>
            {
                var uns = units.Where(i => wnd.Result.Contains(i.Key)).Select(i => i.Value);
                employees.Where(i => uns.Contains(i.SerializedJobPlacement.SerializedUnit)).ToList().ForEach(i =>
                {
                    if (!Employees.Any(j => j.Id == i.Id)) Employees.Add(i);
                });
            };
            wnd.ShowDialog();
        }

        private void AddTodayEmployeesClick(object sender, RoutedEventArgs e)
        {
            EmployeeDropdownButton.IsOpen = false;
            try
            {
                _context.GetEmployeesWorkingAt(DateTime.Today).ForEach(i =>
                {
                    if (!Employees.Any(j => j.Id == i.Id)) Employees.Add(i);
                });
            }
            catch
            {
                ExtraWindow.Alert("Ошибка", "При добавлении произошла ошибка.\nПопробуйте добавить сотрудников по более избирательному критерию.");
            }
        }

        private void AddTomorrowEmployeesClick(object sender, RoutedEventArgs e)
        {
            EmployeeDropdownButton.IsOpen = false;
            try
            {
                _context.GetEmployeesWorkingAt(DateTime.Today.AddDays(1)).ForEach(i =>
                {
                    if (!Employees.Any(j => j.Id == i.Id)) Employees.Add(i);
                });
            }
            catch
            {
                ExtraWindow.Alert("Ошибка", "При добавлении произошла ошибка.\nПопробуйте добавить сотрудников по более избирательному критерию.");
            }
        }
    }
}
