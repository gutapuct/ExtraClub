using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.Interfaces;

namespace ExtraClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    /// <summary>
    /// Interaction logic for CategoryWindow.xaml
    /// </summary>
    public partial class CategoryWindow
    {
        public List<EmployeeCategoryView> Employees { get; set; }

        private IEnumerable<Job> _jobs;
        private Dictionary<Guid, EmployeeCategory> _categories;
        private Dictionary<Guid, Employee> _employees;

        IReportManager _repMan;

        public CategoryWindow(IReportManager repMan, Guid catId)
        {
            InitializeComponent();
            _jobs = _context.GetJobs();
            _categories = _context.GetEmployeeCategories().ToDictionary(i => i.Id, i => i);
            Employees = new List<EmployeeCategoryView>();
            _employees = _context.GetEmployees(true, false).ToDictionary(i => i.Id, i => i);
            foreach (var i in _employees.Values)
            {
                if (catId != Guid.Empty && i.SerializedJobPlacement.CategoryId != catId) continue;
                var ecv = new EmployeeCategoryView
                {
                    IsChecked = false,
                    Category = i.SerializedJobPlacement.SerializedCategoryName,
                    Id = i.Id,
                    Name = i.SerializedCustomer.FullName,
                    Categories = GetCategories(i.SerializedJobPlacement.JobId, i.SerializedJobPlacement.CategoryId)
                };
                if (ecv.Categories.Count > 0)
                {
                    Employees.Add(ecv);
                }
            }

            _repMan = repMan;
            DataContext = this;
        }

        private Dictionary<Guid, string> GetCategories(Guid jobId, Guid without)
        {
            var res = new Dictionary<Guid, string>();
            var job = _jobs.SingleOrDefault(i => i.Id == jobId);
            if (job == null) return res;
            foreach (var i in job.SerializedCategoryIds.Where(j => j != without))
            {
                if (_categories.ContainsKey(i))
                {
                    res.Add(i, _categories[i].Name);
                }
            }
            return res;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var em in Employees.Where(i=>i.IsChecked))
            {
                var empl = _employees[em.Id];
                if (empl.SerializedJobPlacement.CategoryId != em.SelectedCategoryId && em.SelectedCategoryId != Guid.Empty)
                {
                    var res = _context.PostEmployeeCategoryChange(em.Id, em.SelectedCategoryId);
                    if (PrintTripOrder.IsChecked ?? false)
                    {
                        _repMan.ProcessPdfReport(() => _context.GenerateEmployeeCategoryOrder(res));
                    }
                }
            }
            DialogResult = true;
            Close();
        }
    }

    public class EmployeeCategoryView
    {
        public bool IsChecked { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public Dictionary<Guid, string> Categories { get; set; }
        public Guid SelectedCategoryId { get; set; }
        public string Category { get; set; }
    }
}
