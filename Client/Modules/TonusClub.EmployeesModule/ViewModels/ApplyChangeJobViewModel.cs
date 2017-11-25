using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using TonusClub.ServiceModel;
using System.Windows.Data;
using TonusClub.UIControls;

namespace TonusClub.EmployeesModule.ViewModels
{
    class ApplyChangeJobViewModel : INotifyPropertyChanged
    {
        public Employee Employee { get; set; }
        public ICollectionView Jobs { get; set; }
        public ICollectionView Categories { get; set; }

        private readonly List<EmployeeCategory> _categories;
        private readonly List<Job> _jobs;

        public decimal Salary
        {
            get
            {
                if (_currentJob == null || JobPlacement.CategoryId == Guid.Empty) return 0;
                var cat = _categories.SingleOrDefault(i => i.Id == JobPlacement.CategoryId);
                if (cat == null) return 0;
                return _currentJob.Salary * cat.SalaryMulti;
            }
        }

        private JobPlacement _jobPlacement;
        public JobPlacement JobPlacement
        {
            get
            {
                return _jobPlacement;
            }
            set
            {
                _jobPlacement = value;
                OnPropertyChanged("JobPlacement");
            }
        }

        Job _currentJob;
        readonly ClientContext _context;

        public ApplyChangeJobViewModel(Employee employee, ClientContext context)
        {
            _context = context;
            Employee = employee;

            _jobs = context.GetAvailableJobs();
            Jobs = CollectionViewSource.GetDefaultView(_jobs);
            _categories = context.GetEmployeeCategories();
            Categories = CollectionViewSource.GetDefaultView(_categories);
            Categories.Filter = delegate(object item)
            {
                if (!(item is EmployeeCategory)) return false;
                if (CurrentJob == null) return false;
                return CurrentJob.SerializedCategoryIds.Contains(((EmployeeCategory)item).Id);
            };
            if (Employee.SerializedHasJobPlacementDraft)
            {
                JobPlacement = context.GetJobPlacementDraft(employee.Id);
                CurrentJob = _jobs.SingleOrDefault(i => i.Id == JobPlacement.JobId);
            }
            else
            {
                JobPlacement = new JobPlacement
                {
                    ApplyDate = DateTime.Today,
                    AuthorId = context.CurrentUser.UserId,
                    CompanyId = context.CurrentCompany.CompanyId,
                    CreatedOn = DateTime.Now,
                    EmployeeId = employee.Id,
                    TestPeriod = 3,
                    Study = 4,
                    IsAsset = false
                };
            }
            JobPlacement.PropertyChanged += JobPlacement_PropertyChanged;
        }

        void JobPlacement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CategoryId")
            {
                OnPropertyChanged("Salary");
            }
        }

        public Job CurrentJob
        {
            get
            {
                return _currentJob;
            }
            set
            {
                _currentJob = value;
                OnPropertyChanged("CurrentJob");
                JobPlacement.JobId = _currentJob.Id;
                OnPropertyChanged("Salary");
                Categories.Refresh();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        internal bool AssetApply()
        {
            if (!String.IsNullOrEmpty(JobPlacement.Error)) return false;
            JobPlacement.IsAsset = true;
            JobPlacement.Id = _context.PostJobPlacement(JobPlacement);
            return true;
        }

        internal bool AssetChange()
        {
            if (!String.IsNullOrEmpty(JobPlacement.Error)) return false;
            JobPlacement.IsAsset = true;
            JobPlacement.Id = _context.PostJobPlacementChange(JobPlacement);
            return true;
        }

        internal void RemoveJob(Guid jobId)
        {
            var j = _jobs.SingleOrDefault(i => i.Id == jobId);
            if (j != null)
            {
                _jobs.Remove(j);
            }
            Jobs.Refresh();
        }
    }
}
