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
using System.Globalization;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Windows;
using Telerik.Windows.Controls;
using TonusClub.UIControls;

namespace TonusClub.OrganizerModule.Views.Ankets.Windows
{
    public partial class NewAnketDialog
    {
        public List<Division> Divisions { get; set; }
        public KeyValuePair<int?, string>[] Scores { get; set; }
        public KeyValuePair<int?, string>[] Scores2 { get; set; }

        public Anket Anket { get; set; }

        public bool SaveEnabled { get; set; }
        public bool ClubChange { get; set; }

        public NewAnketDialog(ClientContext context, Guid anketId, bool isReadonly)
            : base(context)
        {
            SaveEnabled = !isReadonly;

            InitializeComponent();

            Anket anket;
            if (anketId != Guid.Empty)
            {
                anket = context.GetAnket(anketId);
            }
            else
            {
                anket = new Anket();
            }


            if (ClubChange = anket.Id == Guid.Empty)
            {
                Anket = context.GenerateAnketDefault(context.CurrentDivision.Id, new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(-1).Month, 1));
            }
            else
            {
                Anket = anket;
            }

            Anket.PropertyChanged += Anket_PropertyChanged;

            Divisions = context.GetDivisions();

            CultureInfo cultureInfo = new CultureInfo("ru-RU");
            DateTimeFormatInfo dateInfo = new DateTimeFormatInfo();
            dateInfo.ShortDatePattern = "MMMM yyyy";
            cultureInfo.DateTimeFormat = dateInfo;
            AnketPeriodPicker.Culture = cultureInfo;
            cultureInfo.DateTimeFormat.MonthNames = new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь", "" };
            cultureInfo.DateTimeFormat.AbbreviatedMonthNames = new string[] { "Янв", "Фев", "Мар", "Апр", "Май", "Июн", "Июл", "Авг", "Сен", "Окт", "Ноя", "Дек", "" };

            Scores = new KeyValuePair<int?, string>[]
            {
                new KeyValuePair<int?,string>(null, "Укажите оценку"),
                new KeyValuePair<int?,string>(0, "Отлично"),
                new KeyValuePair<int?,string>(1, "Хорошо"),
                new KeyValuePair<int?,string>(2, "Удовлетворительно"),
                new KeyValuePair<int?,string>(3, "Неудовлетворительно"),
                new KeyValuePair<int?,string>(4, "Плохо")
            };
            
            Scores2 = new KeyValuePair<int?, string>[]
            {
                new KeyValuePair<int?,string>(null, "Укажите оценку"),
                new KeyValuePair<int?,string>(-1, "Не взаимодействовали"),
                new KeyValuePair<int?,string>(0, "Отлично"),
                new KeyValuePair<int?,string>(1, "Хорошо"),
                new KeyValuePair<int?,string>(2, "Удовлетворительно"),
                new KeyValuePair<int?,string>(3, "Неудовлетворительно"),
                new KeyValuePair<int?,string>(4, "Плохо")
            };

            DataContext = this;
        }

        void Anket_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Period" || e.PropertyName == "DivisionId")
            {
                Anket.PropertyChanged -= Anket_PropertyChanged;
                Anket = _context.GenerateAnketDefault(Anket.DivisionId, Anket.Period);
                OnPropertyChanged("Anket");
                Anket.PropertyChanged += Anket_PropertyChanged;
            }
        }

        private void Draft_Click(object sender, RoutedEventArgs e)
        {
            Anket.StatusId = 0;
            _context.PostAnket(Anket);
            DialogResult = true;
            Close();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Anket.Error))
            {
                TonusWindow.Alert("Ошибка", "Необходимо заполнить все обязательные поля анкеты!");
                return;
            };
            Anket.StatusId = 1;
            _context.PostAnket(Anket);
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
