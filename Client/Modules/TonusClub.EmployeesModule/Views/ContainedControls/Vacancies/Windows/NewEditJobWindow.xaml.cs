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
using System.Windows.Shapes;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Vacancies.Windows
{
    public partial class NewEditJobWindow
    {
        public Job Job{ get; set; }

        public List<SalaryScheme> Schemes { get; set; }

        public IEnumerable<EmployeeCategory> Categories { get; set; }

        public List<string> Units { get; set; }

        public NewEditJobWindow(Job job, ClientContext context) : base(context)
        {
            Owner = Application.Current.MainWindow;

            Categories = _context.GetEmployeeCategories();

            Units = _context.GetJobUnits();

            Schemes = _context.GetSalarySchemes();

            if (job == null || job.Id == Guid.Empty)
            {
                Job = new Job
                {
                    CompanyId = context.CurrentCompany.CompanyId,
                    DivisionId = context.CurrentDivision.Id,
                    IsMainWorkplace = true
                };
            }
            else
            {
                Job = job;
                foreach (var tt in Categories)
                {
                    if (Job.SerializedCategoryIds.Contains(tt.Id))
                    {
                        tt.Helper = true;
                    }
                }
            }

            this.DataContext = this;

            InitializeComponent();

        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Job.Name)) return;
            if (String.IsNullOrWhiteSpace(Job.WorkGraph)) return;

            var l = new List<Guid>();
            foreach (var i in Categories)
            {
                if (i.Helper) l.Add(i.Id);
            }

            _context.PostJob(Job, l);

            DialogResult = true;
            Close();
        }
    }
}
