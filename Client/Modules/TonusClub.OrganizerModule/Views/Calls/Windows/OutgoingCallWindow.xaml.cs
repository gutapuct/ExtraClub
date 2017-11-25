using Microsoft.Practices.Unity;
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
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.OrganizerModule.Views.Calls.Windows
{
    public partial class OutgoingCallWindow
    {
        public DateTime StartAt { get; set; }
        public Visibility CanConsult { get; set; }
        string log;
        public string Log
        {
            get
            {
                return log;
            }
            set
            {
                log = value;
                OnPropertyChanged("Log");
                OnPropertyChanged("SaveEnabled");
            }
        }
        public Guid CustomerId { get; set; }
        public string Phone2 { get; set; }

        string goal;
        public string Goal
        {
            get
            {
                return goal;
            }
            set
            {
                goal = value;
                OnPropertyChanged("Goal");
                OnPropertyChanged("SaveEnabled");
            }
        }
        public List<string> Goals { get; set; }

        string result;
        public string Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
                OnPropertyChanged("Result");
                OnPropertyChanged("SaveEnabled");
            }
        }
        public List<string> Results { get; set; }


        public bool SaveEnabled
        {
            get
            {
                return CustomerId != Guid.Empty && !String.IsNullOrWhiteSpace(Log) && !String.IsNullOrWhiteSpace(Result);
            }
        }

        public OutgoingCallWindow(Guid customerId)
        {
            Results = new List<string> {"Куплен абонемент",
                "Клиент записан на консультацию",
                "Клиент записан на процедуру",
                "Не берет трубку",
                "Отказ",
                "Ошибка",
                "Запланирован следующий звонок",
                "Прочее"
            };
            Goals = new List<string> {"Поздравить с ДР",
                "Информирование об акции, мероприятии",
                "Записать на занятие",
                "Информация о действующем абонементе",
                "Кончается абонемент",
                "Долго не ходила, записать на тренировку",
                "Забыла вещи в клубе",
                "Была на пробном",
                "Кончился абонемент, продать"
            };
            InitializeComponent();
            if (customerId != Guid.Empty) CustomerSearch.SelectById(customerId);

            CustomerId = customerId;
            Log = "";
            StartAt = DateTime.Now;
            CanConsult = System.Windows.Visibility.Collapsed;
            DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostNewCall(Log, ServiceModel.Organizer.CallResult.OK, CustomerId, false, StartAt, Goal, Result);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CustomerSearch_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            var cust = _context.GetCustomer(e.Guid);
            CustomerId = e.Guid;
            Phone2 = cust.Phone2;
            OnPropertyChanged("Phone2");
            OnPropertyChanged("SaveEnabled");
#if BEAUTINIKA
            if (cust.ActiveCard == null)
            {
                CanConsult = Visibility.Visible;
                OnPropertyChanged("CanConsult");
            }
#endif
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationManager.MakeScheduleRequest(new Infrastructure.ScheduleRequestParams { Customer = new Customer { Id = CustomerId } });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
#if BEAUTINIKA
            NavigationManager.MakeConsultationRequest(new Infrastructure.ScheduleRequestParams { Customer = _context.GetCustomer(CustomerId) });
#endif
        }
    }
}
