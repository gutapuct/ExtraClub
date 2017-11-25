using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using TonusClub.OrganizerModule.Business;
using TonusClub.ServiceModel;
using System.ComponentModel;
using Telerik.Windows.Controls;
using TonusClub.UIControls;
using TonusClub.UIControls.Windows;
using TonusClub.OrganizerModule.Views.Tasks.Windows.NewCallMaster;

namespace TonusClub.OrganizerModule.Views.Calls.Windows
{
    /// <summary>
    /// Interaction logic for IncomingNewCustomerWindow.xaml
    /// </summary>
    public partial class IncomingNewCustomerWindow : IDataErrorInfo, INotifyPropertyChanged
    {
        public IncomingResult Result { get; private set; }
        public List<File> Links { get; set; }
        public Company Company { get; set; }
        public List<Corporate> Corporates { get; set; }

        public List<string> WorkPlaces { get; set; }
        public List<string> Positions { get; set; }


        public ICollectionView AdvertTypes { get; set; }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool SmsList { get; set; }
        public string AdvertComment { get; set; }
        public Guid RecommendedById { get; set; }
        public string Comments { get; set; }

        public int? SocialStatusId { get; set; }
        public string WorkPlace { get; set; }
        public string Position { get; set; }
        public string WorkPhone { get; set; }
        public int? Kids { get; set; }

        public bool AddTreatments { get; set; }
        public bool AddSolarium { get; set; }

        private Guid _AdvertTypeId;
        public Guid AdvertTypeId
        {
            get
            {
                return _AdvertTypeId;
            }
            set
            {
                _AdvertTypeId = value;
                OnPropertyChanged("AdvertTypeId");

                var type = (AdvertTypes.SourceCollection as IEnumerable<AdvertType>).FirstOrDefault(i => i.Id == _AdvertTypeId);
                if (type == null) return;
                AdvertCommentBox.GetBindingExpression(RadMaskedTextBox.ValueProperty).UpdateSource();
                CustomerSearch.IsEnabled = type.InvitorNeeded;
            }
        }

        public List<StatusView> CurrentStatuses { get; set; }

        ClientContext _context;

        public IncomingNewCustomerWindow(ClientContext context)
        {
            _context = context;
            InitializeComponent();
            Result = IncomingResult.Cancelled;
            Links = context.GetDivisionFiles().Where(i => i.Category == 0).ToList();
            Company = context.CurrentCompany;
            DataContext = this;
            CustomerSearch.IsEnabled = false;
            var tmp = context.GetWorkData();
            WorkPlaces = tmp[0];
            Positions = tmp[1];

            CurrentStatuses = context.GetAllStatuses().OrderBy(i => i.Value).Select(x => new StatusView() { Id = x.Key, IsChecked = false, Name = x.Value }).ToList();
            AdvertTypes = CollectionViewSource.GetDefaultView(context.GetAdvertTypes().OrderBy(t => t.Name));
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Error))
            {
                TonusWindow.Alert("Ошибка", "Необходимо заполнить все обязательные поля!");
                return;
            }
            Result = IncomingResult.SaveClicked;
            Close();
        }

        private void NotACustomerClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.NotACustomer;
            Close();
        }

        private void OldCustomerClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.OldCustomer;
            Close();
        }

        private void LinkClicked(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is File)
            {
                var res = _context.DownloadFile(((FrameworkElement)sender).DataContext as File);
                System.Diagnostics.Process.Start(res);
            }
        }

        private void CustomerSearch_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            RecommendedById = e.Guid;
        }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (!String.IsNullOrEmpty(propertyError))
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                return error.ToString();
            }
        }

        public string this[string columnName]
        {
            get {
                if (columnName == "AdvertTypeId")
                {
                    if (AdvertTypeId == Guid.Empty) return "!";
                }
                var at = (AdvertTypes.SourceCollection as IEnumerable<AdvertType>).FirstOrDefault(i => i.Id == _AdvertTypeId);
                if (columnName == "AdvertComment" && (at == null || (at.CommentNeeded && String.IsNullOrWhiteSpace(AdvertComment)))) return "!";
                if (columnName == "CustomerSearch" &&(at == null || (at.InvitorNeeded && RecommendedById == Guid.Empty))) return "!";
                return "";
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
