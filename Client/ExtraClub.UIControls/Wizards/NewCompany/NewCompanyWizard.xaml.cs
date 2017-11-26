using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ExtraClub.ServiceModel;
using System.ComponentModel;
using ExtraClub.UIControls;

namespace ExtraClub.ClientDal.Wizards.NewCompany
{
    public partial class NewCompanyWizard : INotifyPropertyChanged
    {
        private bool _CanBack = false;
        public bool CanBack
        {
            get
            {
                return _CanBack;
            }
            set
            {
                _CanBack = value;
                OnPropertyChanged("CanBack");
            }

        }
        
        public bool CanFwd
        {
            get
            {
                return ValidateForward();
            }
        }

        public bool CanFinish
        {
            get
            {
                return ValidateData();
            }
        }

        int _CurrentPage = 1;
        public int CurrentPage
        {
            get
            {
                return _CurrentPage;
            }
            set
            {
                _CurrentPage = value;
                Text1.Background = value == 1 ? Brushes.CornflowerBlue : Brushes.Transparent;
                Text2.Background = value == 2 ? Brushes.CornflowerBlue : Brushes.Transparent;
                Text3.Background = value == 3 ? Brushes.CornflowerBlue : Brushes.Transparent;
                CommonTab.Visibility = Visibility.Collapsed;
                CardsTab.Visibility = System.Windows.Visibility.Collapsed;
                TicketsTab.Visibility = System.Windows.Visibility.Collapsed;
                if (value == 1)
                {
                    CommonTab.Visibility = System.Windows.Visibility.Visible;
                }
                if (value == 2)
                {
                    CardsTab.Visibility = System.Windows.Visibility.Visible;
                }
                if (value == 3)
                {
                    TicketsTab.Visibility = System.Windows.Visibility.Visible;
                }
                CanBack = value > 1;
                OnPropertyChanged("CurrentPage");
                OnPropertyChanged("CanFwd");
                OnPropertyChanged("CanFinish");
            }
        }

        public Company Company { get; set; }
        public List<CustomerCardType> Cards { get; set; }
        public List<TicketType> Tickets { get; set; }
        ClientContext _context;
        public NewCompanyWizard(ClientContext context)
        {
            _context = context;
           DataContext = this;
           Company = context.CurrentCompany;
           Company.PropertyChanged += new PropertyChangedEventHandler(Company_PropertyChanged);
           Cards = context.GetCustomerCardTypes(false).Where(i=>i.Id != Guid.Empty).OrderBy(i=>i.Name).ToList();
           Cards.ForEach(i => { i.PropertyChanged += Company_PropertyChanged; });
           Tickets = context.GetTicketTypes(false).OrderBy(i => i.Name).ToList();
           Tickets.ForEach(i => { i.PropertyChanged += Ticket_PropertyChanged; });
           InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Company.PropertyChanged -= new PropertyChangedEventHandler(Company_PropertyChanged);
           Cards.ForEach(i => { i.PropertyChanged -= Company_PropertyChanged; });
           Tickets.ForEach(i => { i.PropertyChanged -= Ticket_PropertyChanged; });

            base.OnClosing(e);
        }

        void Company_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("CanFwd");
        }

        void Ticket_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("CanFinish");
        }

        private bool ValidateData()
        {
            var chk = Tickets.Where(i => i.Helper);
#if BEAUTINIKA
            if (chk.Any(i => (!i.IsGuest) && (!i.IsVisit))) return true;
#else
            if (chk.Any(i => i.IsVisit) && chk.Any(i => i.IsGuest) && chk.Any(i => (!i.IsGuest) && (!i.IsVisit))) return true;
#endif

            return false;
        }

        private bool ValidateForward()
        {
            if (CurrentPage == 1)
            {
                if (!String.IsNullOrWhiteSpace(Company.GeneralManagerName) &&
                    !String.IsNullOrWhiteSpace(Company.GeneralManagerRod) &&
                    !String.IsNullOrWhiteSpace(Company.GeneralManagerPos) &&
                    !String.IsNullOrWhiteSpace(Company.CompanyName) &&
                    !String.IsNullOrWhiteSpace(Company.OrgForm) &&
                    !String.IsNullOrWhiteSpace(Company.INN) &&
                    !String.IsNullOrWhiteSpace(Company.KPP) &&
                    !String.IsNullOrWhiteSpace(Company.KSBank) &&
                    !String.IsNullOrWhiteSpace(Company.BIK) &&
                    !String.IsNullOrWhiteSpace(Company.BankName) &&
                    !String.IsNullOrWhiteSpace(Company.BankCity) &&
                    !String.IsNullOrWhiteSpace(Company.GeneralManagerBaseRod) &&
                    !String.IsNullOrWhiteSpace(Company.RSBank) &&
                    !String.IsNullOrWhiteSpace(Company.Phone1)) return true;
            }
            if (CurrentPage == 2)
            {
                var chk = Cards.Where(i=>i.Helper);
#if BEAUTINIKA
                if (chk.Any(i => (!i.IsGuest) && (!i.IsVisit))) return true;
#else
                if (chk.Any(i => i.IsVisit) && chk.Any(i => i.IsGuest) && chk.Any(i => (!i.IsGuest) && (!i.IsVisit))) return true;
#endif
            }
            return false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage--;
        }

        private void FwdButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage++;
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostCompany(Company);
            Cards.Where(i=>i.Helper).ToList().ForEach(i => _context.PostCompanyCardTypeEnable(i.Id, true));
            Tickets.Where(i=>i.Helper).ToList().ForEach(i => _context.PostCompanyTicketTypeEnable(i.Id, true));
            DialogResult = true;
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
