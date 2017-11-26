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
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.EmployeesModule.Views.ContainedControls.Schedules.Windows
{
    /// <summary>
    /// Interaction logic for AddHolidayWindow.xaml
    /// </summary>
    public partial class AddHolidayWindow
    {

        private DateTime _Date;
        public DateTime Date
        {
            get
            {
                return _Date;
            }
            set
            {
                _Date = value;
                OnPropertyChanged("Date");
            }
        }

        public AddHolidayWindow(ClientContext context)
            : base(context)
        {
            Date = new DateTime(DateTime.Today.Year, 1, 1);
            DataContext = this;
            InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostHoliday(Date);
            DialogResult = true;
            Close();
        }
    }
}
