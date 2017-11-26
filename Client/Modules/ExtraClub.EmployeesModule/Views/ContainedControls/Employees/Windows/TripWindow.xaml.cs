using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ExtraClub.UIControls.Interfaces;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    public partial class TripWindow
    {
        public List<ListItem> Employees { get; set; }

        readonly IReportManager _repMan;


        private DateTime _beginDate;
        public DateTime BeginDate
        {
            get
            {
                return _beginDate;
            }
            set
            {
                _beginDate = value;
                OnPropertyChanged("BeginDate");
                OnPropertyChanged("IllLength");
            }
        }


        private DateTime _endDate;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
                OnPropertyChanged("IllLength");
            }
        }

        public string Destination { get; set; }
        public string Base { get; set; }
        public string Target { get; set; }

        public TripWindow(IReportManager repMan)
        {
            _repMan = repMan;
            InitializeComponent();
            Employees = new List<ListItem>();
            foreach (var e in _context.GetEmployees(true, false))
            {
                Employees.Add(new ListItem { IsChecked = false, Name = e.SerializedCustomer.FullName, Value = e.Id });
            }
            BeginDate = DateTime.Today.AddDays(1);
            EndDate = DateTime.Today.AddDays(5);
            DataContext = this;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            var emps = Employees.Where(i => i.IsChecked).Select(i => i.Value).ToArray();
            if (emps.Length == 0)
            {
                ExtraWindow.Alert("Ошибка", "Необходимо выбрать хотя бы одного сотрудника!");
                return;
            }
            if (EndDate < BeginDate)
            {
                ExtraWindow.Alert("Ошибка", "Дата окончания не может быть ранее даты начала!");
                return;
            }
            if (String.IsNullOrEmpty(Destination) || String.IsNullOrEmpty(Base) || String.IsNullOrEmpty(Target))
            {
                ExtraWindow.Alert("Ошибка", "Необходимо заполнить все поля!");
                return;
            }
            var res = _context.PostEmployeeTrip(emps, BeginDate, EndDate, Destination, Target, Base);
            if (PrintTripOrder.IsChecked ?? false)
            {
                foreach (var i in res)
                {
                    _repMan.ProcessPdfReport(() => _context.GenerateEmployeeTripOrder(i));
                }
            }
            DialogResult = true;
            Close();
        }
    }

    public class ListItem
    {
        public Guid Value { get; set; }
        public bool IsChecked { get; set; }
        public string Name { get; set; }
    }
}
