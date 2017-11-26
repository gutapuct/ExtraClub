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
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;
using ExtraClub.UIControls.BaseClasses;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.TurnoverModule.Views.ContainedControls.Windows
{
    public partial class NewIncomeWindow
    {
        public List<IncomeType> IncomeTypes { get; set; }

        public List<string> PaymentTypes { get; set; }


        public Income Income { get; set; }
        
        public bool IsCommon
        {
            get
            {
                return Income != null && !Income.DivisionId.HasValue;
            }
            set
            {
                if (value)
                {
                    Income.DivisionId = null;
                }
                else
                {
                    Income.DivisionId = _context.CurrentDivision.Id;
                }
                OnPropertyChanged("IsCommon");
            }
        }

        public NewIncomeWindow(ClientContext context, Income income)
        {

            PaymentTypes = new List<string> { "нал", "безнал", "касса" };

            IncomeTypes = context.GetDivisionIncomeTypes();
            if (income == null || income.Id == Guid.Empty)
            {
                Income = new Income
                {
                    CompanyId = context.CurrentCompany.CompanyId,
                    CreatedOn = DateTime.Now,
                    DivisionId = context.CurrentDivision.Id
                };
            }
            else
            {
                Income = ViewModelBase.Clone<Income>(income);
            }
            InitializeComponent();
            DataContext = this;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            string message = null;
            if (Income.IncomeTypeId == Guid.Empty) message = UIControls.Localization.Resources.CategoryNeeded;
            if (Income.Amount <= 0) message = UIControls.Localization.Resources.SumNeeded;
            if (String.IsNullOrWhiteSpace(Income.Name)) message = UIControls.Localization.Resources.TitleNeeded;
            if (String.IsNullOrWhiteSpace(Income.PaymentType)) message = UIControls.Localization.Resources.PaymentWayNeeded;
            if (message != null)
            {
                ExtraWindow.Alert(UIControls.Localization.Resources.Error, message);
                return;
            }

            _context.PostIncome(Income);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
