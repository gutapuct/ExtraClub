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
using Microsoft.Practices.Unity;
using TonusClub.OrganizerModule.Business;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;
using TonusClub.UIControls;
using System.ComponentModel;

namespace TonusClub.OrganizerModule.Views.Calls.Windows
{
    public partial class IncomingOldCustomerWindow : INotifyPropertyChanged
    {
        public IncomingResult Result { get; set; }
        public List<File> Links { get; set; }
        public Company Company { get; set; }
        public string Comments { get; set; }

        private Customer _Customer;
        public Customer Customer
        {
            get
            {
                return _Customer;
            }
            set
            {
                _Customer = value;
                OnPropertyChanged("Customer");
            }
        }

        ClientContext _context;

        public IncomingOldCustomerWindow(ClientContext context)
        {
            _context = context;
            InitializeComponent();
            Company = context.CurrentCompany;
            Result = IncomingResult.Cancelled;
            Links = context.GetDivisionFiles().Where(i => i.Category == 1).ToList();

            DataContext = this;
        }

        private void NewTreatmentsClick(object sender, RoutedEventArgs e)
        {
            NavigationManager.MakeScheduleRequest(new Infrastructure.ScheduleRequestParams { Customer = Customer });
        }

        private void TreatmentsWindowClick(object sender, RoutedEventArgs e)
        {
            NavigationManager.MakeActivateTreatmentsScheduleRequest();
        }

        private void NewSolariumClick(object sender, RoutedEventArgs e)
        {
            NavigationManager.MakeNewSolariumVisitRequest(new Infrastructure.ParameterClasses.NewSolariumVisitParams
            {
                CustomerId = Customer == null ? Guid.Empty : Customer.Id,
                StartDate = DateTime.Today.AddDays(1).AddHours(12)
            });
        }

        private void SolariumWindowClick(object sender, RoutedEventArgs e)
        {
            NavigationManager.MakeActivateSolariumScheduleRequest();
        }

        private void CustomerCardClick(object sender, RoutedEventArgs e)
        {
            if (Customer != null)
            {
                NavigationManager.MakeClientRequest(Customer.Id);
            }
        }

        private void NewCustomerClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.NewCustomer;
            Close();
        }

        private void NewScrenarioClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.NewCustomerScrenario;
            Close();
        }

        private void NotACustomerClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.NotACustomer;
            Close();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.SaveClicked;
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.Cancelled;
            Close();
        }

        private void CustomerSearch_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            Customer = _context.GetCustomer(e.Guid);
        }

        private void LinkClicked(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is File)
            {
                var res = _context.DownloadFile(((FrameworkElement)sender).DataContext as File);
                System.Diagnostics.Process.Start(res);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
