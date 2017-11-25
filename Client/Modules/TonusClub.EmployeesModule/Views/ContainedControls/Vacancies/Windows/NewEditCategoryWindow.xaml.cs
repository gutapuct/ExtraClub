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
    /// <summary>
    /// Interaction logic for NewEditCategoryWindow.xaml
    /// </summary>
    public partial class NewEditCategoryWindow
    {
        private IDictionaryManager _dictMan;

        public EmployeeCategory Category { get; set; }

        public IEnumerable<Job> Jobs { get; set; }


        public NewEditCategoryWindow(IDictionaryManager dictMan, ClientContext context, EmployeeCategory category):base(context)
        {
            Owner = Application.Current.MainWindow;
            _dictMan = dictMan;

            Jobs = _context.GetJobs();

            if (category == null || category.Id == Guid.Empty)
            {
                Category = new EmployeeCategory
                {
                    CompanyId = context.CurrentCompany.CompanyId
                };
            }
            else
            {
                Category = category;
                foreach (var tt in Jobs)
                {
                    if (Category.SerializedJobIds.Contains(tt.Id))
                    {
                        tt.Helper = true;
                    }
                }
            }

            this.DataContext = this;

            InitializeComponent();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Category.Name)) return;

            var l = new List<Guid>();
            foreach (var i in Jobs)
            {
                if (i.Helper) l.Add(i.Id);
            }

            _context.PostEmployeeCategory(Category, l);

            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
