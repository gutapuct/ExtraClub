using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.ClientDal.Wizards.NewClub
{
    public partial class NewDivisionWizard : INotifyPropertyChanged
    {
        private bool _CanBack;
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
                Text4.Background = value == 4 ? Brushes.CornflowerBlue : Brushes.Transparent;
                CommonTab.Visibility = Visibility.Collapsed;
                TreatmentTab.Visibility = Visibility.Collapsed;
                SolariumsTab.Visibility = Visibility.Collapsed;
                StoresTab.Visibility = Visibility.Collapsed;
                if (value == 1)
                {
                    CommonTab.Visibility = Visibility.Visible;
                }
                if (value == 2)
                {
                    TreatmentTab.Visibility = Visibility.Visible;
                }
                if (value == 3)
                {
                    SolariumsTab.Visibility = Visibility.Visible;
                }
                if (value == 4)
                {
                    StoresTab.Visibility = Visibility.Visible;
                }
                CanBack = value > 2;
                OnPropertyChanged("CurrentPage");
                OnPropertyChanged("CanFwd");
                OnPropertyChanged("CanFinish");
            }
        }

        public Division Division { get; set; }
        public List<Treatment> _Treatments { get; set; }
        public ICollectionView Treatments { get; set; }

        public List<Solarium> _Solariums { get; set; }
        public ICollectionView Solariums { get; set; }

        public List<Storehouse> _Storehouses { get; set; }
        public ICollectionView Storehouses { get; set; }

        public ClientContext Context { get { return _context; } }
        ClientContext _context;
        public NewDivisionWizard(ClientContext context)
        {
            _context = context;
            _Treatments = new List<Treatment>();
            Treatments = CollectionViewSource.GetDefaultView(_Treatments);
            _Solariums = new List<Solarium>();
            Solariums = CollectionViewSource.GetDefaultView(_Solariums);
            _Storehouses = new List<Storehouse>();
            Storehouses = CollectionViewSource.GetDefaultView(_Storehouses);
            DataContext = this;
            Division = new Division { CompanyId = context.CurrentCompany.CompanyId, AuthorId = context.CurrentUser.UserId, Id = Guid.NewGuid() };
            Division.PropertyChanged += Company_PropertyChanged;
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Division.PropertyChanged -= Company_PropertyChanged;
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
            return false;
        }

        private bool ValidateForward()
        {
            if (CurrentPage == 1)
            {
                if (!String.IsNullOrWhiteSpace(Division.Name) &&
                    !String.IsNullOrWhiteSpace(Division.CityName) &&
                    !String.IsNullOrWhiteSpace(Division.Street) &&
                    Division.OpenDate.HasValue &&
                    Division.PresellDate.HasValue &&
                    Division.OpenTime.HasValue &&
                    Division.CloseTime.HasValue &&
                    Division.OpenTime < Division.CloseTime) return true;
            }
            if (CurrentPage == 2 || CurrentPage == 3) return true;
            return false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage--;
        }

        private void FwdButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage == 1)
            {
                _context.PostNewDivision(Division);
                CancelBtn.Content = "Завершить";
            }
            CurrentPage++;
        }

        internal void RefreshTreatments()
        {
            _Treatments.Clear();
            _Treatments.AddRange(_context.GetAllTreatments(Division.Id));
            Treatments.Refresh();
        }

        internal void RefreshSolariums()
        {
            _Solariums.Clear();
            _Solariums.AddRange(_context.GetDivisionSolariums(Division.Id));
            Solariums.Refresh();
        }

        internal void RefreshStorehouses()
        {
            _Storehouses.Clear();
            _Storehouses.AddRange(_context.GetStorehouses(Division.Id));
            Storehouses.Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}