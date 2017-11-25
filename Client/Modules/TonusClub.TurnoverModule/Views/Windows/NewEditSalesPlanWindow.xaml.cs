using System;
using System.Windows;
using System.Globalization;
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.TurnoverModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for NewEditSalesPlanWindow.xaml
    /// </summary>
    public partial class NewEditSalesPlanWindow
    {

        private DateTime _Month;
        public DateTime Month
        {
            get
            {
                return _Month;
            }
            set
            {
                _Month = value;
                OnPropertyChanged("Month");
            }
        }

        public DateTime OldMonth { get; set; }


        private decimal _Amount;
        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                _Amount = value;
                OnPropertyChanged("Amount");
            }
        }


        private decimal _AmountCorp;
        public decimal AmountCorp
        {
            get
            {
                return _AmountCorp;
            }
            set
            {
                _AmountCorp = value;
                OnPropertyChanged("AmountCorp");
            }
        }

        public NewEditSalesPlanWindow(ClientContext context, SalesPlan plan)
            : base(context)
        {
            InitializeComponent();
            if (plan.Id != Guid.Empty)
            {
                OldMonth = Month = plan.Month;
                Amount = plan.Value;
                AmountCorp = plan.CorpValue;
            }
            else
            {
                Month = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
                OldMonth = DateTime.MinValue;
            }

            CultureInfo cultureInfo = new CultureInfo("ru-RU");
            DateTimeFormatInfo dateInfo = new DateTimeFormatInfo();
            dateInfo.ShortDatePattern = "MMMM yyyy";
            //dateInfo.ShortTimePattern = "HH:mm";
            cultureInfo.DateTimeFormat = dateInfo;
            GenPicker.Culture = cultureInfo;
            cultureInfo.DateTimeFormat.MonthNames = new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь", "" };
            cultureInfo.DateTimeFormat.AbbreviatedMonthNames = new string[] { "Янв", "Фев", "Мар", "Апр", "Май", "Июн", "Июл", "Авг", "Сен", "Окт", "Ноя", "Дек", "" };


            DataContext = this;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostSalaryPlan(Month, Amount, AmountCorp, OldMonth);
            DialogResult = true;
            Close();
        }
    }
}
