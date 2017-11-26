using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using ExtraClub.ServiceModel.Employees;
using ExtraClub.UIControls.Interfaces;

namespace ExtraClub.EmployeesModule.Views.ContainedControls.Schedules.Windows
{
    public partial class NewWorkScheduleWindow
    {

        public ICollectionView ResultView { get; set; }
        readonly List<EmployeeWorkScheduleItem> _result = new List<EmployeeWorkScheduleItem>();

        public ICollectionView Dates { get; set; }
        readonly List<string> _dates = new List<string>();

        private DateTime _start;
        public DateTime Start
        {
            get
            {
                return _start;
            }
            set
            {
                _start = value;
                OnPropertyChanged("Start");
            }
        }

        private DateTime _finish;
        public DateTime Finish
        {
            get
            {
                return _finish;
            }
            set
            {
                _finish = value;
                OnPropertyChanged("Finish");
            }
        }

        readonly IReportManager _repMan;



        public NewWorkScheduleWindow(IReportManager repMan)
        {
            _repMan = repMan;
            InitializeComponent();
            ResultView = CollectionViewSource.GetDefaultView(_result);
            Dates = CollectionViewSource.GetDefaultView(_dates);
            Start = DateTime.Today.AddDays(1);
            Finish = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);
            DataContext = this;
        }

        private void GenerateScheduleClick(object sender, RoutedEventArgs e)
        {
            var holidays = _context.GetHolidays(Start.Year);
            _result.Clear();
            _dates.Clear();
            for (var i = 0; i <= (Finish - Start).TotalDays; i++)
            {
                _dates.Add(Start.AddDays(i).ToString("d.MM"));
            }
            Dates.Refresh();
            var sch = _context.GetEmployeeWorkSchedule(Start, Finish).OrderBy(i => i.JobPlacement.SerializedUnit).ThenBy(i => i.JobPlacement.SerializedJobName).ThenBy(i => i.JobPlacement.SerializedFullName);
            _result.AddRange(sch);
            _result.AsParallel().ForAll(i => i.Init());
            _result.AsParallel().ForAll(i =>
            {
                i.Dates.Values.ToList().ForEach(j => j.IsHoliday = holidays.Contains(j.Date.Date));
            });
            ResultView.Refresh();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            var dict = _result.ToDictionary(i => i.JobPlacement.Id, i => i.Dates.Where(j => j.Value.IsSet).Select(j => j.Value.Date).OrderBy(j => j).ToList());

            var res = _context.PostEmployeeSchedule(dict);

            if ((PrintBox.IsChecked ?? false) && res != Guid.Empty)
            {
                _repMan.ProcessPdfReport(() => _context.GenerateEmployeeScheduleReport(res));
            }

            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
