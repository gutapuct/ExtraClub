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
using TonusClub.UIControls.Windows;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;
using System.Collections.ObjectModel;
using Microsoft.Practices.Unity;
using TonusClub.OrganizerModule.Views.Tasks.Windows.NewCallMaster;
using TonusClub.UIControls;
using TonusClub.Infrastructure;

namespace TonusClub.OrganizerModule.Views.Tasks.Windows
{
    public partial class NewTaskWindow
    {
        public ObservableCollection<Employee> Employees { get; set; }
        public string Subject { get; set; }
        public string Comments { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int Priority { get; set; }

        public Dictionary<int, string> Priorities { get; set; }

        List<Employee> employees;


        public NewTaskWindow()
        {
            Priorities = new Dictionary<int, string>();
            Priorities.Add(0, "Высочайший");
            Priorities.Add(1, "Высокий");
            Priorities.Add(2, "Обычный");
            Priorities.Add(3, "Низкий");

            Employees = new ObservableCollection<Employee>();
            ExpiryDate = DateTime.Now.AddDays(1);
            employees = _context.GetEmployees(true, true);
            Priority = 2;
            Comments = "";
            InitializeComponent();
            DataContext = this;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostNewTask(Employees.Select(i => i.Id).ToArray(), Subject, Comments, ExpiryDate, Priority);
            DialogResult = true;
            Close();
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
                TonusWindow.Alert("Ошибка", "При добавлении произошла ошибка.\nПопробуйте добавить сотрудников по более избирательному критерию.");
            }
        }

        private void AddInJobEmployeesClick(object sender, RoutedEventArgs e)
        {
            ModuleViewBase.ProcessUserDialog<SelectCustomerStatusesWindow>(
                ApplicationDispatcher.UnityContainer,
                wnd =>
                {
                    if (wnd.DialogResult ?? false)
                    {

                        employees.Where(i => wnd.Result.Contains(i.SerializedJobPlacement.JobId)).ToList().ForEach(i =>
                        {
                            if (!Employees.Any(j => j.Id == i.Id)) Employees.Add(i);
                        });
                    }
                },
            wnd =>
            {
                wnd.AppendCollection<Job>(_context.GetJobs(), i => i.Id, i => i.Name);
            });
        }

        private void AddInUnitEmployeesClick(object sender, RoutedEventArgs e)
        {
            var units = employees.Select(i => i.SerializedJobPlacement.SerializedUnit).Distinct().ToDictionary(i => Guid.NewGuid(), i => i);

            ModuleViewBase.ProcessUserDialog<SelectCustomerStatusesWindow>(ApplicationDispatcher.UnityContainer, wnd =>
            {
                if (wnd.DialogResult ?? false)
                {
                    var uns = units.Where(i => wnd.Result.Contains(i.Key)).Select(i => i.Value);
                    employees.Where(i => uns.Contains(i.SerializedJobPlacement.SerializedUnit)).ToList().ForEach(i =>
                    {
                        if (!Employees.Any(j => j.Id == i.Id)) Employees.Add(i);
                    });
                }
            },
            wnd =>
            {
                wnd.AppendCollection<KeyValuePair<Guid, string>>(units, i => i.Key, i => i.Value);
            });


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
                TonusWindow.Alert("Ошибка", "При добавлении произошла ошибка.\nПопробуйте добавить сотрудников по более избирательному критерию.");
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
                TonusWindow.Alert("Ошибка", "При добавлении произошла ошибка.\nПопробуйте добавить сотрудников по более избирательному критерию.");
            }
        }

        private void RemoveEmployeeClick(object sender, RoutedEventArgs e)
        {
            var empl = (sender as FrameworkElement).DataContext as Employee;
            if (empl == null) return;
            Employees.Remove(empl);
        }
    }
}
