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
using TonusClub.ServiceModel;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.UIControls.Windows;
using Telerik.Windows.Controls;
using TonusClub.ServiceModel.Organizer;
using TonusClub.UIControls;
using TonusClub.ServiceModel.Reports;

namespace TonusClub.OrganizerModule.Views.Tasks.Windows
{
    /// <summary>
    /// Interaction logic for CustomerNotificationTaskWindow.xaml
    /// </summary>
    public partial class CustomerNotificationTaskWindow
    {
        public CustomerNotification Notify { get; set; }
        public CustomerNotificationInfo Customer { get; set; }
        public string Result { get; set; }
        public List<String> Results { get; set; }
        public Visibility CanConsult { get; set; }

        public CustomerNotificationTaskWindow(CustomerNotification item)
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

            Notify = item;
            Customer = _context.GetCustomerNotificationInfo(item.CustomerId);
            CanConsult = System.Windows.Visibility.Collapsed;
            DataContext = this;
            InitializeComponent();
        }

        private void CompletedClick(object sender, RoutedEventArgs e)
        {
            _context.PostNotificationCompletion(Notify.Id, Notify.CompletionComment, Result);
            DialogResult = true;
            Close();
        }

        private void IncorrectPhoneClick(object sender, RoutedEventArgs e)
        {
            TonusWindow.Prompt("Неверный номер",
                "Укажите подробности",
                "",
                wnd =>
                {
                    if ((wnd.DialogResult ?? false))
                    {
                        _context.PostIncorrectPhoneTask(Notify.Id, Customer.Id, wnd.TextResult);
                        DialogResult = true;
                        Close();
                    }
                });
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationManager.MakeScheduleRequest(new Infrastructure.ScheduleRequestParams { Customer = new Customer { Id = Customer.Id } });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
        }
    }
}
