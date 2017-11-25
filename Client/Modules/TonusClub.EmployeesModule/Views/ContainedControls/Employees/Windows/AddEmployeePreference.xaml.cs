using System;
using System.Collections.Generic;
using System.Windows;
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    /// <summary>
    /// Interaction logic for AddEmployeePreference.xaml
    /// </summary>
    public partial class AddEmployeePreference
    {
        public Dictionary<short, string> Types { get; private set; }
        public Employee Employee { get; set; }

        private DateTime _BeginDate;
        public DateTime BeginDate
        {
            get
            {
                return _BeginDate;
            }
            set
            {
                _BeginDate = value;
                OnPropertyChanged("BeginDate");
            }
        }

        private DateTime _EndDate;
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                _EndDate = value;
                OnPropertyChanged("EndDate");
            }
        }


        private short _CurrentType;
        public short CurrentType
        {
            get
            {
                return _CurrentType;
            }
            set
            {
                _CurrentType = value;
                OnPropertyChanged("CurrentType");
            }
        }

        public AddEmployeePreference(ClientContext context, Employee employee)
            : base(context)
        {
            Types = new Dictionary<short, string>();
            Types.Add(0, "Отпуск");
            Types.Add(1, "Командировка");
            Types.Add(2, "Отгул");

            BeginDate = DateTime.Today.AddDays(1);
            EndDate = DateTime.Today.AddDays(2);

            Employee = employee;

            this.DataContext = this;

            InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostEmployeePreference(Employee.Id, BeginDate, EndDate, CurrentType);
            DialogResult = true;
            Close();
        }
    }
}
