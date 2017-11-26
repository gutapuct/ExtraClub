using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.ServiceModel;
using Microsoft.Practices.Unity;
using System.Windows.Data;
using ExtraClub.Infrastructure;
using ExtraClub.ServiceModel.Employees;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.UIControls;
using ExtraClub.UIControls.BaseClasses;
using ExtraClub.UIControls.Interfaces;

namespace ExtraClub.EmployeesModule.ViewModels
{
    public class EmployeesLargeViewModel : ViewModelBase
    {
        public ICollectionView CategoriesView { get; set; }
        private List<EmployeeCategory> _CategoriesView = new List<EmployeeCategory>();

        public ICollectionView EmployeesView { get; set; }
        internal List<Employee> _EmployeesView = new List<Employee>();

        public ICollectionView JobsView { get; set; }
        private List<Job> _JobsView = new List<Job>();

        public ICollectionView EmployeeDocumentsView { get; set; }
        private List<EmployeeDocument> _EmployeeDocumentsView = new List<EmployeeDocument>();

        public ICollectionView VacationsHistoryView { get; set; }
        private List<VacationList> _VacationsHistoryView = new List<VacationList>();

        public ICollectionView CurrentVacationsScheduleView { get; set; }
        private List<EmployeeScheduleProposalElement> _CurrentVacationsScheduleView = new List<EmployeeScheduleProposalElement>();

        public ICollectionView WorkGraphsView { get; set; }
        private List<EmployeeWorkGraph> _WorkGraphsView = new List<EmployeeWorkGraph>();

        public ICollectionView HolidaysView { get; set; }
        private List<DateTime> _HolidaysView = new List<DateTime>();

        public ICollectionView SalarySchemesView { get; set; }
        private List<SalaryScheme> _SalarySchemesView = new List<SalaryScheme>();

        public ICollectionView SalariesView { get; set; }
        private List<SalarySheet> _SalariesView = new List<SalarySheet>();

        public ICollectionView EmployeeCashlowView { get; set; }
        private List<EmployeePayment> _EmployeeCashlowView = new List<EmployeePayment>();

        public bool CanEmployeeBeActivated
        {
            get
            {
                if (EmployeesView.CurrentItem == null) return false;
                var se = EmployeesView.CurrentItem as Employee;
                return (se.SerializedJobPlacement == null || se.SerializedJobPlacement.FireDate != null) && !se.IsActive;
            }
        }

        public bool CanEmployeeBeDeactivated
        {
            get
            {
                if (EmployeesView.CurrentItem == null) return false;
                var se = EmployeesView.CurrentItem as Employee;
                return (se.SerializedJobPlacement == null || se.SerializedJobPlacement.FireDate != null) && se.IsActive;
            }
        }

        bool _ShowActiveEmpsOnly = true;
        public bool ShowActiveEmpsOnly
        {
            get
            {
                return _ShowActiveEmpsOnly;
            }
            set
            {
                _ShowActiveEmpsOnly = value;
                if (value)
                {
                    EmployeesView.Filter = e =>
                    {
                        var em = e as Employee;
                        return em.IsActive;
                    };
                }
                else
                {
                    EmployeesView.Filter = null;
                }
                EmployeesView.Refresh();
            }
        }


        private DateTime vacsStart = new DateTime(DateTime.Today.Year, 1, 1);
        public DateTime VacsStart
        {
            get
            {
                return vacsStart;
            }
            set
            {
                vacsStart = value;
                if (vacsStart > VacsEnd)
                {
                    vacsStart = vacsEnd;
                }
                OnPropertyChanged("VacsStart");
                CurrentVacationsScheduleView.Refresh();
            }
        }

        private DateTime vacsEnd = new DateTime(DateTime.Today.Year, 12, 31);
        public DateTime VacsEnd
        {
            get
            {
                return vacsEnd;
            }
            set
            {
                vacsEnd = value;
                if (VacsStart > vacsEnd)
                {
                    vacsEnd = vacsStart;
                }
                OnPropertyChanged("VacsEnd");
                CurrentVacationsScheduleView.Refresh();
            }
        }

        private bool _IsNotBaselined;
        public bool IsNotBaselined
        {
            get
            {
                return _IsNotBaselined;
            }
            set
            {
                _IsNotBaselined = value;
                OnPropertyChanged("IsNotBaselined");
            }
        }

        private string _BaselineStatus;
        public string BaselineStatus
        {
            get
            {
                return _BaselineStatus;
            }
            set
            {
                _BaselineStatus = value;
                OnPropertyChanged("BaselineStatus");
            }
        }

        public EmployeesLargeViewModel(IUnityContainer container)
            : base()
        {
            CategoriesView = CollectionViewSource.GetDefaultView(_CategoriesView);
            JobsView = CollectionViewSource.GetDefaultView(_JobsView);
            EmployeesView = CollectionViewSource.GetDefaultView(_EmployeesView);
            EmployeesView.CurrentChanged += EmployeesView_CurrentChanged;
            EmployeesView.Filter = e =>
            {
                var em = e as Employee;
                return em.IsActive;
            };

            EmployeeDocumentsView = CollectionViewSource.GetDefaultView(_EmployeeDocumentsView);
            VacationsHistoryView = CollectionViewSource.GetDefaultView(_VacationsHistoryView);
            CurrentVacationsScheduleView = CollectionViewSource.GetDefaultView(_CurrentVacationsScheduleView);

            CurrentVacationsScheduleView.Filter = delegate(object item)
            {
                if (item == null) return false;
                var spe = ((EmployeeScheduleProposalElement)item);
                return VacsEnd >= spe.Start && spe.Finish >= VacsStart;
            };

            WorkGraphsView = CollectionViewSource.GetDefaultView(_WorkGraphsView);
            HolidaysView = CollectionViewSource.GetDefaultView(_HolidaysView);
            SalarySchemesView = CollectionViewSource.GetDefaultView(_SalarySchemesView);
            SalariesView = CollectionViewSource.GetDefaultView(_SalariesView);
            EmployeeCashlowView = CollectionViewSource.GetDefaultView(_EmployeeCashlowView);
        }

        void EmployeesView_CurrentChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("CanEmployeeBeActivated");
            OnPropertyChanged("CanEmployeeBeDeactivated");
        }

        protected override void RefreshDataInternal()
        {
            _CategoriesView.Clear();
            _CategoriesView.AddRange(ClientContext.GetEmployeeCategories());
            _JobsView.Clear();
            _JobsView.AddRange(ClientContext.GetJobs());
            _EmployeesView.Clear();
            _EmployeesView.AddRange(ClientContext.GetEmployees(false, false));
            _EmployeeDocumentsView.Clear();
            _EmployeeDocumentsView.AddRange(ClientContext.GetEmployeeDocuments());
            _VacationsHistoryView.Clear();
            _VacationsHistoryView.AddRange(ClientContext.GetVacationHistory());
            _CurrentVacationsScheduleView.Clear();
            _CurrentVacationsScheduleView.AddRange(ClientContext.GetCurrentEmployeeVacationsSchedule());
            _WorkGraphsView.Clear();
            _WorkGraphsView.AddRange(ClientContext.GetWorkGraphs());
            _HolidaysView.Clear();
            _HolidaysView.AddRange(ClientContext.GetHolidays(DateTime.Today.Year));
            _SalarySchemesView.Clear();
            _SalarySchemesView.AddRange(ClientContext.GetSalarySchemes());
            _SalariesView.Clear();
            _SalariesView.AddRange(ClientContext.GetSalarySheets());
            _EmployeeCashlowView.Clear();
            _EmployeeCashlowView.AddRange(ClientContext.GetDivisionEmployeeCashflow());
            RefreshBaselineStatus();
        }

        internal void RefreshBaselineStatus()
        {
            BaselineStatus = ClientContext.GetBaselineStatus();
            IsNotBaselined = String.IsNullOrEmpty(BaselineStatus);
        }

        protected override void RefreshFinished()
        {
            base.RefreshFinished();

            CategoriesView.Refresh();
            JobsView.Refresh();
            EmployeesView.Refresh();
            EmployeeDocumentsView.Refresh();
            VacationsHistoryView.Refresh();
            CurrentVacationsScheduleView.Refresh();
            WorkGraphsView.Refresh();
            HolidaysView.Refresh();
            SalarySchemesView.Refresh();
            SalariesView.Refresh();
            EmployeeCashlowView.Refresh();
        }

        internal void RefreshSalarySchemes() {
            _SalarySchemesView.Clear();
            _SalarySchemesView.AddRange(ClientContext.GetSalarySchemes());
            SalarySchemesView.Refresh();
        }
        internal void RefreshEmployeeCashlow()
        {
            _EmployeeCashlowView.Clear();
            _EmployeeCashlowView.AddRange(ClientContext.GetDivisionEmployeeCashflow());
            EmployeeCashlowView.Refresh();
        }


        internal void RefreshSalarySheets() {
            _SalariesView.Clear();
            _SalariesView.AddRange(ClientContext.GetSalarySheets());
            SalariesView.Refresh();
        }

        internal void RefreshEmployees()
        {
            _EmployeesView.Clear();
            _EmployeesView.AddRange(ClientContext.GetEmployees(false, false));
            EmployeesView.Refresh();
        }

        internal void RefreshHolidays()
        {
            _HolidaysView.Clear();
            _HolidaysView.AddRange(ClientContext.GetHolidays(DateTime.Today.Year));
            HolidaysView.Refresh();
        }

        internal void RefreshVacationHistory()
        {
            _VacationsHistoryView.Clear();
            _VacationsHistoryView.AddRange(ClientContext.GetVacationHistory());
            _CurrentVacationsScheduleView.Clear();
            _CurrentVacationsScheduleView.AddRange(ClientContext.GetCurrentEmployeeVacationsSchedule());

            VacationsHistoryView.Refresh();
            CurrentVacationsScheduleView.Refresh();
        }

        internal void RefreshDocuments()
        {
            _EmployeeDocumentsView.Clear();
            _EmployeeDocumentsView.AddRange(ClientContext.GetEmployeeDocuments());
            EmployeeDocumentsView.Refresh();
            RefreshEmployeeCashlow();
        }

        internal void RefreshCategories()
        {
            _CategoriesView.Clear();
            _CategoriesView.AddRange(ClientContext.GetEmployeeCategories());
            CategoriesView.Refresh();
        }

        internal void RefreshJobs()
        {
            _JobsView.Clear();
            _JobsView.AddRange(ClientContext.GetJobs());
            JobsView.Refresh();
            RefreshBaselineStatus();
        }

        internal void RefreshWorkGraphs()
        {
            _WorkGraphsView.Clear();
            _WorkGraphsView.AddRange(ClientContext.GetWorkGraphs());
            WorkGraphsView.Refresh();
        }

        internal void PrintEmployeeDocuments()
        {
            ApplicationDispatcher.UnityContainer.Resolve<IReportManager>().ProcessPdfReport((ClientContext.GenerateEmployeeDocumentsList));
        }

        internal void SetEmployeeActive(bool active)
        {
            var e = EmployeesView.CurrentItem as Employee;
            if (e == null) return;
            ClientContext.PostEmployeeActive(e.Id, active);
            e.IsActive = active;
            EmployeesView.Refresh();
        }
    }
}
