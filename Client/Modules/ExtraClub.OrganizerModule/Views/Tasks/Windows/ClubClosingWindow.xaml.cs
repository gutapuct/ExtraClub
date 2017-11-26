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
using ExtraClub.UIControls.Windows;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;

namespace ExtraClub.OrganizerModule.Views.Tasks.Windows
{
    /// <summary>
    /// Interaction logic for ClubClosingWindow.xaml
    /// </summary>
    public partial class ClubClosingWindow
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Cause { get; set; }

        public ClubClosingWindow()
        {
            Start = DateTime.Now.AddDays(1);
            End = DateTime.Now.AddDays(1).AddHours(1);
            DataContext = this;
            InitializeComponent();
#if BEAUTINIKA
            Title = "Закрытие студии";
#endif
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommitClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Cause))
            {
                ExtraWindow.Alert("Ошибка", "Укажите причину закрытия, которая будет записана в задачи на обзвон клиентов!");
                return;
            }
            _context.PostClubClosing(Start, End, Cause);
            DialogResult = true;
            Close();
        }
    }
}
