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
using System.Globalization;
using System.Threading;
using ExtraClub.UIControls;

namespace ExtraClub.TurnoverModule.Views.ContainedControls.Windows
{
    public partial class NewEditDivisionFinanceWindow
    {
        public DivisionFinance Finance { get; set; }

        public NewEditDivisionFinanceWindow(ClientContext context, DivisionFinance fin)
        {
            if (fin.Id == Guid.Empty)
            {
                fin.Period = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }
            Finance = fin;

            InitializeComponent();
            if (Thread.CurrentThread.CurrentCulture.Name.Contains("ru"))
            {
                CultureInfo cultureInfo = new CultureInfo("ru-RU");
                DateTimeFormatInfo dateInfo = new DateTimeFormatInfo();
                dateInfo.ShortDatePattern = "MMMM yyyy";
                //dateInfo.ShortTimePattern = "HH:mm";
                cultureInfo.DateTimeFormat = dateInfo;
                GenPicker.Culture = cultureInfo;
                cultureInfo.DateTimeFormat.MonthNames = new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь", "" };
                cultureInfo.DateTimeFormat.AbbreviatedMonthNames = new string[] { "Янв", "Фев", "Мар", "Апр", "Май", "Июн", "Июл", "Авг", "Сен", "Окт", "Ноя", "Дек", "" };
            }
            DataContext = this;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostDivisionFinance(Finance.Period, Finance.CashLeft, Finance.Unsent, Finance.Advances, Finance.TerminalLoan, Finance.Accum);
            DialogResult = true;
            Close();
        }
    }
}
