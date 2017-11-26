using System;
using System.Collections.Generic;
using System.Windows;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.BaseClasses;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.TurnoverModule.Views.Windows
{
    public partial class NewSpendingWindow
    {

        public List<SpendingType> SpendingTypes { get; set; }

        public List<string> PaymentTypes { get; set; }

        public Spending Spending { get; set; }

        public bool IsCommon
        {
            get
            {
                return Spending != null && !Spending.DivisionId.HasValue;
            }
            set
            {
                if (value)
                {
                    Spending.DivisionId = null;
                }
                else
                {
                    Spending.DivisionId = _context.CurrentDivision.Id;
                }
                OnPropertyChanged("IsCommon");
            }
        }

        public NewSpendingWindow(Spending spending)
        {
            PaymentTypes = new List<string> { "нал", "безнал", "касса" };
            SpendingTypes = _context.GetDivisionSpendingTypes();
            if (spending == null || spending.Id == Guid.Empty)
            {
                Spending = new Spending
                {
                    CompanyId = _context.CurrentCompany.CompanyId,
                    CreatedOn = DateTime.Now,
                    DivisionId = _context.CurrentDivision.Id
                };
            }
            else
            {
                Spending = ViewModelBase.Clone<Spending>(spending);
            }
            InitializeComponent();
            DataContext = this;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (Spending.SpendingTypeId == Guid.Empty) return;
            if (Spending.Amount <= 0) return;
            if (String.IsNullOrWhiteSpace(Spending.Name)) return;
            if (String.IsNullOrWhiteSpace(Spending.PaymentType)) return;

            if (Spending.IsFinAction)
            {
                if (Spending.IsInvestment)
                {
                    ExtraWindow.Alert("Ошибка", "Нельзя одновременно указать признаки Инвестиция и Фин.деятельность!");
                    return;
                }
            }

            _context.PostSpending(Spending);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
