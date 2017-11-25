using System;
using System.Windows;
using TonusClub.ServiceModel;
using System.ServiceModel;
using TonusClub.UIControls;
using TonusClub.UIControls.Interfaces;
using TonusClub.UIControls.Windows;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    /// <summary>
    /// Interaction logic for MissWindow.xaml
    /// </summary>
    public partial class MissWindow
    {
public Employee Employee { get; set; }
        IReportManager _repMan;


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
                OnPropertyChanged("MissLength");
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
                OnPropertyChanged("MissLength");
           }
        }

        public int MissLength
        {
            get
            {
                return (int)((EndDate - BeginDate).TotalDays + 1);
            }
        }

        public MissWindow(ClientContext context, Employee employee, IReportManager repMan)
            : base(context)
        {
            _repMan = repMan;
            Employee = employee;
            BeginDate = DateTime.Today.AddDays(1);
            EndDate = DateTime.Today.AddDays(14);
            DataContext = this;
            InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            try {
                var vacId = _context.PostEmployeeVacation(Employee.Id, BeginDate, EndDate, 2, 0, 0, null);

            DialogResult = true;
            Close();
            }
            catch (FaultException ex)
            {
                TonusWindow.Alert("Ошибка", ex.Message);
            }

        }
    }
}
