using Microsoft.Practices.Unity;
using System;
using System.Windows;
using ExtraClub.ServiceModel.Reports;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.Reports.ViewModels
{
    public class WorkbenchReportViewModel : ViewModelBase
    {
        private const string WorkbenchVisitsTemplateText = "Запланированные на {0} посещения";
        private const string WorkbenchCallsTemplateText = "Запланированные на {0} звонки";
        private const string WorkbenchTasksTemplateText = "Задачи на {0}";

        private DateTime _workbenchDateTime = DateTime.Today;

        public Visibility LicenseVisibility { get; set; }
        public string LicenceText { get; set; }

        public string Width1 { get; set; }
        public string Width2 { get; set; }
        public string PlanText { get; set; }

        public string WorkbenchVisitsText
        {
            get
            {
                if (_workbenchDateTime == DateTime.Today)
                {
                    return string.Format(WorkbenchVisitsTemplateText, "сегодня");
                }
                return string.Format(WorkbenchVisitsTemplateText, "завтра");
            }
        }

        public string WorkbenchCallsText
        {
            get
            {
                if (_workbenchDateTime == DateTime.Today)
                {
                    return string.Format(WorkbenchCallsTemplateText, "сегодня");
                }
                return string.Format(WorkbenchCallsTemplateText, "завтра");
            }
        }

        public string WorkbenchTasksText
        {
            get
            {
                if (_workbenchDateTime == DateTime.Today)
                {
                    return string.Format(WorkbenchTasksTemplateText, "сегодня");
                }
                return string.Format(WorkbenchTasksTemplateText, "завтра");
            }
        }

        public WorkbenchInfo WorkbenchInfo { get; set; }

        public bool WorkbenchReportToday
        {
            get { return _workbenchDateTime == DateTime.Today; }
            set
            {
                _workbenchDateTime = DateTime.Today;
                OnPropertyChanged("WorkbenchReportToday");
                OnPropertyChanged("WorkbenchReportTomorrow");
                RefreshDataAsync();
            }
        }

        public bool WorkbenchReportTomorrow
        {
            get { return _workbenchDateTime == DateTime.Today.AddDays(1); }
            set
            {
                _workbenchDateTime = DateTime.Today.AddDays(1);
                OnPropertyChanged("WorkbenchReportToday"); 
                OnPropertyChanged("WorkbenchReportTomorrow");
                RefreshDataAsync();
            }
        }

        public WorkbenchReportViewModel(IUnityContainer container)
            : base()
        {
            LicenseVisibility = Visibility.Collapsed;
        }

        protected override void RefreshDataInternal()
        {
            //var ls = ClientContext.GetLocalSettings();
            //if(ls != null)
            //{
            //    var lic = ls.LicenseExpiry;
            //    if(lic.HasValue && DateTime.Today.AddDays(7) >= lic)
            //    {
            //        LicenceText = String.Format("Лицензия на использование Flagmax Direction истекает {0:d MMMM yyyy}", lic.Value);
            //        LicenseVisibility = Visibility.Visible;
            //        OnPropertyChanged("LicenceText");
            //    }
            //}
            //else
            {
                LicenseVisibility = Visibility.Collapsed;
            }
            OnPropertyChanged("LicenseVisibility");

            WorkbenchInfo = ClientContext.GetWorkbench(_workbenchDateTime);
            OnPropertyChanged("WorkbenchInfo");

            if(WorkbenchInfo.SalesPlan == 0)
            {
                Width1 = "1*";
                Width1 = "1*";
                PlanText = "План на текущий месяц не задан";
            }
            else
            {
                var plan = Math.Floor(WorkbenchInfo.TotalSales / WorkbenchInfo.SalesPlan * 100);
                if(plan > 100)
                {
                    PlanText = String.Format("План перевыполнен на {0:n0}%", plan - 100);
                    Width1 = "1*";
                    Width2 = "0*";
                }
                else
                {
                    Width1 = plan.ToString() + "*";
                    Width2 = (100 - plan).ToString() + "*";
                    PlanText = String.Format("План выполнен на {0:n0}% (осталось {1:n0} р.)", plan, WorkbenchInfo.SalesPlan - WorkbenchInfo.TotalSales);
                }
            }
            OnPropertyChanged("Width1");
            OnPropertyChanged("Width2");
            OnPropertyChanged("PlanText");



            OnPropertyChanged("WorkbenchVisitsText");
            OnPropertyChanged("WorkbenchCallsText");
            OnPropertyChanged("WorkbenchTasksText");

        }

    }
}
