using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TonusClub.ServiceModel.Employees;
using System.ComponentModel;
using TonusClub.UIControls;
using TonusClub.EmployeesModule.Views.ContainedControls.Employees.Windows;
using TonusClub.Infrastructure;
using TonusClub.UIControls.Interfaces;
using ViewModelBase = TonusClub.UIControls.BaseClasses.ViewModelBase;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Schedules.Windows
{
    public partial class NewVacationListWindow
    {
        private readonly List<EmployeeScheduleProposalElement> _schedule;

        public ICollectionView ScheduleView { get; set; }


        private int _year;
        public int Year
        {
            get
            {
                return _year;
            }
            set
            {
                _year = value;
                OnPropertyChanged("Year");
            }
        }


        private int _recDays;
        public int RecDays
        {
            get
            {
                return _recDays;
            }
            set
            {
                _recDays = value;
                OnPropertyChanged("RecDays");
            }
        }

        readonly IReportManager _repMan;

        public NewVacationListWindow(ClientContext context, IReportManager repMan, List<EmployeeScheduleProposalElement> list, Guid id)
        {
            _repMan = repMan;
            Year = DateTime.Today.Year;
            DataContext = this;
            _schedule = new List<EmployeeScheduleProposalElement>();

            if (list != null)
            {
                foreach (var i in list)
                {
                    _schedule.Add(ViewModelBase.Clone<EmployeeScheduleProposalElement>(i));
                }
            }

            if (id != Guid.Empty)
            {
                foreach (var i in context.GetEmployeeVacationsSchedule(id))
                {
                    _schedule.Add(i);
                }
            }

            ScheduleView = CollectionViewSource.GetDefaultView(_schedule);
            RecDays = 14;

            InitializeComponent();
        }

        private void GenerateScheduleClick(object sender, RoutedEventArgs e)
        {
            if (RecDays <= 1) return;
            _schedule.Clear();
            _schedule.AddRange(_context.GenerateScheduleProposal(Year, RecDays));
            ScheduleView.Refresh();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            var res = _context.PostEmployeeVacationsSchedule(_schedule);
            if ((PrintBox.IsChecked ?? false) && res != Guid.Empty)
            {
                _repMan.ProcessPdfReport(() => _context.GenerateEmployeeVacationList(res));
            }
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RemoveProposalClick(object sender, RoutedEventArgs e)
        {
            var id = (Guid)(((Button) sender).Tag);
            _schedule.Remove(_schedule.Single(i => i.Id == id));
            ScheduleView.Refresh();
        }

        private void AddScheduleLineClick(object sender, RoutedEventArgs e)
        {
            var dlg = ApplicationDispatcher.UnityContainer.Resolve<SelectEmployeeWindow>();
            var dlgResult = dlg.ShowDialog();
            if (dlgResult ?? false)
            {
                _schedule.Insert(0, new EmployeeScheduleProposalElement
                {
                    EmployeeId = dlg.EmployeeResult.Id,
                    EmployeeJob = dlg.EmployeeResult.SerializedJobPlacement.SerializedJobName,
                    EmployeeName = dlg.EmployeeResult.SerializedCustomer.FullName,
                    Unit = dlg.EmployeeResult.SerializedJobPlacement.SerializedUnit,
                    Start = DateTime.Today.AddDays(1),
                    Finish = DateTime.Today.AddDays(2)
                });
                ScheduleView.Refresh();
            }
        }
    }
}
