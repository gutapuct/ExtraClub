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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExtraClub.Reports.Views.ContainedControls;
using ExtraClub.ServiceModel;
using ExtraClub.ServiceModel.Reports;
using ExtraClub.UIControls;

namespace ExtraClub.Reports.Views.RecurrentReports
{
    public partial class NewEditRecurrentRule
    {
        public IEnumerable<ReportInfoInt> Reports { get; set; }
        public Dictionary<int, string> Recurrencies { get; set; }
        public Dictionary<int, string> Periods { get; set; }

        public Guid Id { get; set; }
        public int PeriodDay { get; set; }
        public int SelectedRecurrency { get; set; }
        public int? SelectedPeriod { get; set; }

        private ReportInfoInt _SelectedReport;
        public ReportInfoInt SelectedReport
        {
            get
            {
                return _SelectedReport;
            }
            set
            {
                _SelectedReport = value;
                OnPropertyChanged("SelectedReport");
            }
        }

        public NewEditRecurrentRule(ClientContext context, ReportRecurrency item)
            : base(context)
        {
            Reports = context.GetUserReportsList();
            foreach(var report in Reports)
            {
                if(report.Parameters.Any(i => i.InternalName == "start"))
                {
                    report.HasDatePeriod = true;
                }
                report.Parameters = report.Parameters.Where(i => !(i.InternalName == "start" || i.InternalName == "end")).ToList();
                report.Parameters.ForEach(i => ReportContainerBase.InitParam(context, i));
            }

            Recurrencies = ReportRecurrency.Recurrencies;
            Periods = new Dictionary<int, string>
            {
                {0, "За вчера"},
                {1, "С начала текущего месяца"},
                {2, "За прошлый месяц"},
                {3, "За последние 30 дней"},
                {4, "С начала текущей недели"},
                {5, "За прошлую неделю"},
                {6, "За последние 7 дней"}
            };
            InitializeComponent();

            if(item == null || item.Id==Guid.Empty)
            {
                Id = Guid.NewGuid();
                PeriodDay = 1;
            }
            else
            {
                Id = item.Id;
                PeriodDay = item.PeriodDay;
                SelectedRecurrency = item.Recurrency;
                SelectedReport = Reports.FirstOrDefault(i => i.Key == item.ReportKey);
                //parameters
                var pars = ReportRecurrency.DeserializeParameters(item.Parameters);
                SelectedPeriod = pars.Period;
                if(SelectedReport != null)
                {
                    foreach(var par in pars.Parameters)
                    {
                        var p = SelectedReport.Parameters.FirstOrDefault(i => i.InternalName == par.Item1);
                        if(p != null)
                        {
                            p.InstanceValue = par.Item2;
                        }
                    }
                }
            }


            DataContext = this;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedReport == null) return;

            if(!SelectedReport.HasDatePeriod)
            {
                SelectedPeriod = null;
            }

            _context.PostRecurrentRule(new ReportRecurrency
            {
                CompanyId = _context.CurrentCompany.CompanyId,
                Id = Id,
                Parameters = ReportRecurrency.SerializeParameters(SelectedPeriod, SelectedReport.Parameters),
                PeriodDay = PeriodDay,
                Recurrency = SelectedRecurrency,
                ReportKey = SelectedReport.Key,
                UserId = _context.CurrentUser.UserId
            });
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
