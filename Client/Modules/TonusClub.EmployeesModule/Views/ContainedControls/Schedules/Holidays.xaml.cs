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
using TonusClub.EmployeesModule.ViewModels;
using TonusClub.EmployeesModule.Views.ContainedControls.Schedules.Windows;
using Microsoft.Practices.Unity;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Schedules
{
    /// <summary>
    /// Interaction logic for Holidays.xaml
    /// </summary>
    public partial class Holidays
    {
        private EmployeesLargeViewModel Model
        {
            get
            {
                return this.DataContext as EmployeesLargeViewModel;
            }
        }

        public Holidays()
        {
            InitializeComponent();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<AddHolidayWindow>(() => Model.RefreshHolidays());
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            if (Model.HolidaysView.CurrentItem == null) return;
            ClientContext.DeleteHoliday((DateTime)Model.HolidaysView.CurrentItem);
            Model.RefreshHolidays();
        }
    }
}
