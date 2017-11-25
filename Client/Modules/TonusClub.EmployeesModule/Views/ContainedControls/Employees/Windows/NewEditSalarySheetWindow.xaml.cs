using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Interfaces;
using TonusClub.UIControls.Windows;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Employees.Windows
{
    /// <summary>
    /// Interaction logic for NewEditSalarySheetWindow.xaml
    /// </summary>
    public partial class NewEditSalarySheetWindow
    {
        readonly List<SalarySheetRow> _sheetView = new List<SalarySheetRow>();
        public ICollectionView SheetView { get; set; }

        readonly IReportManager _repMan;
        readonly SalarySheet _sheet;
        public Guid SheetId { get; set; }

        private bool _canSalaryChange;
        public bool CanSalaryChange
        {
            get
            {
                return _canSalaryChange;
            }
            set
            {
                _canSalaryChange = value;
                OnPropertyChanged("CanSalaryChange");
            }
        }

        private DateTime _genDate;
        public DateTime GenDate
        {
            get
            {
                return _genDate;
            }
            set
            {
                _genDate = value.AddDays(-value.Day + 1);
                OnPropertyChanged("GenDate");
            }
        }

        public string TotalPayed
        {
            get
            {
                return _sheetView.Sum(i => i.SerializedAdvance).ToString("c");
            }
        }

        public string TotalToPay
        {
            get
            {
                return _sheetView.Sum(i => i.TotalToPay).ToString("c");
            }
        }

        public NewEditSalarySheetWindow(IReportManager repMan, SalarySheet sheet)
        {
            _repMan = repMan;
            _sheet = sheet;
            InitializeComponent();
            GenDate = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
            SheetView = CollectionViewSource.GetDefaultView(_sheetView);

            var cultureInfo = new CultureInfo("ru-RU");
            var dateInfo = new DateTimeFormatInfo {ShortDatePattern = "MMMM yyyy"};
            cultureInfo.DateTimeFormat = dateInfo;
            GenPicker.Culture = cultureInfo;
            cultureInfo.DateTimeFormat.MonthNames = new[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь", "" };
            cultureInfo.DateTimeFormat.AbbreviatedMonthNames = new[] { "Янв", "Фев", "Мар", "Апр", "Май", "Июн", "Июл", "Авг", "Сен", "Окт", "Ноя", "Дек", "" };

            if(!(CanSalaryChange = sheet.Id == Guid.Empty))
            {
                GenDate = sheet.PeriodStart.AddDays(-sheet.PeriodStart.Day + 1);
                _sheetView.AddRange(_context.GetSalarySheetLines(sheet.Id));
                _sheetView.ForEach(i => i.CanSalaryChange = false);
                SheetView.Refresh();
                SheetId = sheet.Id;
                CancelButton.Content = "Закрыть";
                VedGroup.IsEnabled = false;
                PrintBtn.Visibility = Visibility.Visible;
            }



            DataContext = this;
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            var res = _context.PostSalarySheet(GenDate.AddDays(-GenDate.Day + 1), _sheetView);
            if(String.IsNullOrEmpty(res))
            {
                DialogResult = true;
                Close();
            }
            else
            {
                TonusWindow.Alert("Ошибка", res);
            }
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GenerateClick(object sender, RoutedEventArgs e)
        {
            if(_context.CheckSalarySheet(GenDate))
            {
                TonusWindow.Confirm("Замена ведомости", "За указанной период уже сформирована ведомость.\nПри проведении новой ведомости старая будет перемещена в архив, все выплаты будут привязаны к новой ведомости.\nПродолжить?",
                    w =>
                    {
                        GenPicker.IsEnabled = false;
                        _sheetView.Clear();
                        _sheetView.AddRange(_context.GenerateSalarySheet(GenDate.AddDays(-GenDate.Day + 1)));
                        _sheetView.ForEach(i => i.CanSalaryChange = true);
                        SheetView.Refresh();
                        OnPropertyChanged("TotalToPay");
                        OnPropertyChanged("TotalPayed");
                    });
            }
            else
            {
                GenPicker.IsEnabled = false;
                _sheetView.Clear();
                _sheetView.AddRange(_context.GenerateSalarySheet(GenDate.AddDays(-GenDate.Day + 1)));
                _sheetView.ForEach(i => i.CanSalaryChange = true);
                SheetView.Refresh();
                OnPropertyChanged("TotalToPay");
                OnPropertyChanged("TotalPayed");
            }
        }

        private void ShowLogClick(object sender, RoutedEventArgs e)
        {
            TonusWindow.Alert("Отчет о формировании зарплаты", ((SalarySheetRow)((Button)sender).DataContext).Log);
        }

        private void PayClick(object sender, RoutedEventArgs e)
        {
            var row = (SalarySheetRow)((Button)sender).DataContext;
            if(row.TotalToPay > 0)
            {
                TonusWindow.Prompt(
                        "Введите выдаваемую сумму",
                   $"Укажите сумму (более нуля, не более {row.TotalToPay:c}\n(Все выплаты, меньшие этой суммы, будут сохранены как аванс, равная - как выдача зарплаты):",
                          row.TotalToPay.ToString("n2"),
                            payClosed =>
                            {
                                if(payClosed.DialogResult ?? false)
                                {
                                    decimal amount;
                                    if(Decimal.TryParse(payClosed.TextResult, out amount))
                                    {
                                        if(amount <= Math.Round(row.TotalToPay, 2))
                                        {
                                            var res = _context.PostEmployeePayment(row.EmployeeId, SheetId, amount);
                                            if(!String.IsNullOrEmpty(res))
                                            {
                                                TonusWindow.Alert("Ошибка", res);
                                            }
                                            else
                                            {
                                                RefreshLines();
                                            }
                                        }
                                        else
                                        {
                                            TonusWindow.Alert("Ошибка", "Введенная сумма превышает выплаты по ведомости!");
                                        }
                                    }
                                    else
                                    {
                                        TonusWindow.Alert("Ошибка", "Введенная сумма неверна!");
                                    }
                                }
                            }
                        );
            }
            else
            {
                TonusWindow.Alert("Ошибка", "По данной ведомости сотруднику уже выплачена полная сумма!");
            }
        }

        private void RefreshLines()
        {
            _sheetView.Clear();
            _sheetView.AddRange(_context.GetSalarySheetLines(SheetId));
            _sheetView.ForEach(i => i.CanSalaryChange = false);
            SheetView.Refresh();
            OnPropertyChanged("TotalToPay");
            OnPropertyChanged("TotalPayed");
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            _repMan.ExportObjectToExcel(
               $"Зарплатная ведомость за {_sheet.PeriodStart:MMMM yyyy} по клубу {_context.CurrentDivision.Name}",
                _sheetView,
                new ColumnInfo<SalarySheetRow>("ФИО", i => i.SerializedEmployeeName),
                new ColumnInfo<SalarySheetRow>("Начислено", i => i.Salary),
                new ColumnInfo<SalarySheetRow>("Премия", i => i.Bonus),
                new ColumnInfo<SalarySheetRow>("Итого", i => i.SalaryTotal),
                new ColumnInfo<SalarySheetRow>("НДФЛ", i => i.NDFL),
                new ColumnInfo<SalarySheetRow>("Вед10", i => i.Ved10),
                new ColumnInfo<SalarySheetRow>("Вед25", i => i.Ved25),
                new ColumnInfo<SalarySheetRow>("Выплачено", i => i.SerializedAdvance),
                new ColumnInfo<SalarySheetRow>("Итого к выплате", i => i.TotalToPay)
                );
        }
    }
}
