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
using TonusClub.UIControls;
using TonusClub.Infrastructure.Interfaces;
using System.ComponentModel;
using TonusClub.ServiceModel;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using TonusClub.UIControls.Localization;
using TonusClub.UIControls.Windows;
using System.Threading;
using System.IO;
using TonusClub.Infrastructure;

namespace TonusClub.Clients.Views
{
    public partial class NewCustomerWindow : TonusClub.UIControls.WindowBase
    {
        private IDictionaryManager _dictMan;
        private IUnityContainer _container;
        public List<AdvertType> AdvertTypes { get; set; }
        public List<Corporate> Corporates { get; set; }

        public List<string> WorkPlaces { get; set; }
        public List<string> Positions { get; set; }

        public Guid GuidResult { get; private set; }

        public byte[] ImageBytes;

        public Brush RecomendationBorderBrush
        {
            get
            {
                if (Customer.InvitorId != null) return Brushes.Transparent;
                if (Customer.AdvertTypeId != null)
                {
                    var at = AdvertTypes.FirstOrDefault(i => i.Id == Customer.AdvertTypeId);
                    if (at != null && at.InvitorNeeded) return Brushes.Red;
                }
                return Brushes.Transparent;
            }
        }

        string _newPathText;
        public string NewPathText
        {
            get { return _newPathText; }
            set
            {
                _newPathText = value;
                OnPropertyChanged("NewPathText");
            }
        }

        public Customer Customer { get; set; }

        public string PassportMask { get; set; }
        public MaskType PassportMaskType { get; set; }
        public string PhoneMask { get; set; }

        public NewCustomerWindow(IUnityContainer container, IDictionaryManager dictMan, ClientContext context)
            : base(context)
        {
            NewPathText = UIControls.Localization.Resources.ClickToSelectFile;

            _dictMan = dictMan;
            _container = container;

            Corporates = context.GetCorporates();
            AdvertTypes = context.GetAdvertTypes().OrderBy(t => t.Name).ToList();

            var tmp = context.GetWorkData();
            WorkPlaces = tmp[0];
            Positions = tmp[1];

            Customer = new Customer
            {
                ClubId = context.CurrentDivision.Id,
                Gender = false,
                SmsList = true,
            };


            InitializeComponent();
            LocalizationManager.Manager = new CustomLocalizationManager();
            date1.SelectableDateEnd = DateTime.Today;
            date2.SelectableDateEnd = DateTime.Today;

            DataContext = this;

            Customer.PropertyChanged += new PropertyChangedEventHandler(Customer_PropertyChanged);

            if(AppSettingsManager.GetSetting("Language") == "3")
            {
                PassportMask = "0000000";
                PassportMaskType = MaskType.Standard;
                PhoneMask = "+000 (00) 000 00 00";
                Customer.Phone2 = "988";
            }
            else if(AppSettingsManager.GetSetting("Language") == "2")
            {
                PassportMask = "";
                PassportMaskType = MaskType.None;
                PhoneMask = "+000 00 00 00 00";
                Customer.Phone2 = "357";
            }
            else
            {
                PassportMask = "0000 000 000";
                PassportMaskType = MaskType.Standard;
                PhoneMask = "+0 (000) 000-00-00";
                Customer.Phone2 = "7";
            }


        }

        private void CustomerSearch_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            Customer.InvitorId = e.Guid;
            OnPropertyChanged("RecomendationBorderBrush");
        }

        void Customer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "AdvertTypeId")
            {
                var at = AdvertTypes.FirstOrDefault(t => t.Id == Customer.AdvertTypeId);
                Customer.AdvertType = at;
                AdvertComment.GetBindingExpression(RadMaskedTextBox.ValueProperty).UpdateSource();
            }
            OkButton.IsEnabled = String.IsNullOrEmpty(Customer.Error);
            OkCardButton.IsEnabled = false;
            if(OkButton.IsEnabled)
            {
                if(!String.IsNullOrWhiteSpace(Customer.PasspNumber) && !String.IsNullOrWhiteSpace(Customer.PasspEmitPlace) && Customer.PasspEmitDate.HasValue)
                {
                    OkCardButton.IsEnabled = true;
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            TestByPhone(() =>
            {
                GuidResult = _context.PostCustomer(Customer);

                if(ImageBytes != null && ImageBytes.Length > 0)
                {
                    _context.UpdateCustomerImage(GuidResult, ImageBytes);
                }

                NavigationManager.MakeClientRequest(GuidResult);
                DialogResult = true;
                Close();
            });
        }

        private void TestByPhone(Action onOkay)
        {
            var custId = _context.GetCustomerIdByPhone(Customer.Phone2);
            if(custId != Guid.Empty)
            {
                //TonusWindow.Confirm(TonusClub.UIControls.Localization.Resources.Warning,
                //    TonusClub.UIControls.Localization.Resources.CustomerPhoneWarning, w =>
                //{
                //    if (w.DialogResult ?? false)
                //    {
                //        onOkay();
                //    }
                //    else
                //    {
                //        NavigationManager.MakeClientRequest(custId);
                //        DialogResult = false;
                //        Close();
                //    }
                //});
                TonusWindow.Alert("Предупреждение", "Клиент с таким номером телефона уже существует! Перед Вами раннее созданная карточка этого клиента!");
                NavigationManager.MakeClientRequest(custId);
                DialogResult = false;
                Close();
            }
            else
            {
                onOkay();
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            TonusWindow.Confirm(TonusClub.UIControls.Localization.Resources.Cancel,
                TonusClub.UIControls.Localization.Resources.NewCustomerCancelWarning,
                    e1 =>
                    {
                        if((e1.DialogResult ?? false))
                        {
                            Close();
                        }
                    });
        }

        private void OkCardButton_Click(object sender, RoutedEventArgs e)
        {
            TestByPhone(() =>
            {
                Customer = _context.GetCustomer(_context.PostCustomer(Customer));
                if(ImageBytes != null && ImageBytes.Length > 0)
                {
                    _context.UpdateCustomerImage(Customer.Id, ImageBytes);
                }
                ModuleViewBase.ProcessUserDialog<Windows.CustomerAndCards.NewCustomerCard>(_container, x =>
                {
                    NavigationManager.MakeClientRequest(Customer.Id);
                    DialogResult = true;
                    Close();
                }, new ParameterOverride("customer", Customer));

            });
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".jpg",
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp"
            };


            if(dlg.ShowDialog() ?? false)
            {
                NewPathText = dlg.FileName;
                var file = new FileInfo(dlg.FileName);
                using(var fs = file.OpenRead())
                {
                    ImageBytes = Infrastructure.Imaging.StoreImageWithResize(fs, 300, 300);
                }
            }

        }

        private void RadComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged("RecomendationBorderBrush");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Временный костыль (событие не отрабатывает)
            if ((email.MaskedText ?? "").Length == 0 || !((CheckBox)sender).IsChecked.GetValueOrDefault())
            {
                email.MaskedText = "1";
                email.MaskedText = null;
            }
        }
    }
}
