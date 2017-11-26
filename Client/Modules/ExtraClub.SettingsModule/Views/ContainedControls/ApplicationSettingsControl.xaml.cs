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
using ExtraClub.UIControls;
using System.Configuration;
using ExtraClub.Infrastructure;
using ExtraClub.SettingsModule.ViewModels;
using System.Threading;
using ExtraClub.UIControls.Windows;
using ExtraClub.ServiceModel;
using System.Net;
using Microsoft.Practices.Unity;
using ExtraClub.CashRegisterModule;

namespace ExtraClub.SettingsModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for ApplicationSettingsControl.xaml
    /// </summary>
    public partial class ApplicationSettingsControl : ModuleViewBase
    {
        private SettingsLargeViewModel _model;

        public string AppVersion { get; set; }
        public string ServerAddress { get; set; }
        public bool NoKKM { get; set; }
        public bool Spark { get; set; }
        public bool Pirit { get; set; }
        public bool Atol { get; set; }
        public string KKMPort { get; set; }
        public string KKMPassword { get; set; }
        public int SectionsNumber { get; set; }

        public bool ShowCardNums { get; set; }
        public bool ShowNames { get; set; }
        public bool ShowPhones { get; set; }

        public string Line1KKM { get; set; }
        public string Line2KKM { get; set; }
        public string Line3KKM { get; set; }
        public string Line4KKM { get; set; }

        public decimal Yellow { get; set; }
        public decimal Red { get; set; }
        public decimal Orange { get; set; }

        public Guid CurrentDivision { get; set; }
        public int CurrentLanguage { get; set; }

        public object Divisions { get; set; }
        public object Languages { get; set; }

        private LocalSetting _localSettings;
        public LocalSetting LocalSettings
        {
            get
            {
                return _localSettings;
            }
            set
            {
                _localSettings = value;
                OnPropertyChanged("LocalSettings");
            }
        }

        CashRegisterManager CashMan;

        public ApplicationSettingsControl(SettingsLargeViewModel model, CashRegisterManager cashMan)
        {
            CashMan = cashMan;
            _model = model;
            InitializeComponent();
#if BEAUTINIKA
            clubText.Text = "Студия (потребуется перезапуск)";
#endif
            Divisions = _model.ClientContext.GetDivisions();
            Languages = new Dictionary<int, string> { { 0, "Русский" }, { 1, "English" }, { 2, "Русский (Казахстан)" }, { 3, "Русский (Узбекистан)" } };
            CurrentDivision = Guid.Parse(AppSettingsManager.GetSetting("DivisionId"));

            ClientContext.ExecuteMethodAsync(i => i.GetLocalSettings()).ContinueWith(i =>
              {
                  LocalSettings = i.Result;
              });

            try
            {
                AppVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
                ServerAddress = AppSettingsManager.GetSetting("ServerAddress");
                KKMPort = AppSettingsManager.GetSetting("SparkPortNumber");
                KKMPassword = AppSettingsManager.GetSetting("SparkAccessKey");
                NoKKM = AppSettingsManager.GetSetting("UseKKM") == "0";
                Spark = AppSettingsManager.GetSetting("UseKKM") == "1";
                Pirit = AppSettingsManager.GetSetting("UseKKM") == "2";
                Atol = AppSettingsManager.GetSetting("UseKKM") == "3";
                Line1KKM = AppSettingsManager.GetSetting("Line1KKM");
                Line2KKM = AppSettingsManager.GetSetting("Line2KKM");
                Line3KKM = AppSettingsManager.GetSetting("Line3KKM");
                Line4KKM = AppSettingsManager.GetSetting("Line4KKM");
                Red = Decimal.Parse(AppSettingsManager.GetSetting("Red"));
                Orange = Decimal.Parse(AppSettingsManager.GetSetting("Orange"));
                Yellow = Decimal.Parse(AppSettingsManager.GetSetting("Yellow"));
                CurrentLanguage = Int32.Parse(AppSettingsManager.GetSetting("Language"));
                SectionsNumber = Int32.Parse(AppSettingsManager.GetSetting("SectionsNumber"));
                ShowCardNums = AppSettingsManager.GetSetting("ShowCardNums") == "1";
                ShowNames = AppSettingsManager.GetSetting("ShowNames") == "1";
                ShowPhones = AppSettingsManager.GetSetting("ShowPhones") == "1";
            }
            catch(NullReferenceException) { }

            this.DataContext = this;
        }

        private int GetKKMInt()
        {
            if(Spark) return 1;
            if(Pirit) return 2;
            if(Atol) return 3;
            return 0;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            bool restart = false;
            if(ServerAddress != AppSettingsManager.GetSetting("ServerAddress"))
            {
                AppSettingsManager.SetSetting("ServerAddress", ServerAddress.Trim());
                restart = true;
            }
            AppSettingsManager.SetSetting("SparkPortNumber", KKMPort.Trim());
            AppSettingsManager.SetSetting("SparkAccessKey", KKMPassword);
            if(GetKKMInt().ToString() != AppSettingsManager.GetSetting("UseKKM"))
            {
                AppSettingsManager.SetSetting("UseKKM", GetKKMInt().ToString());
                restart = true;
            }

            if(CurrentLanguage != Int32.Parse(AppSettingsManager.GetSetting("Language")))
            {
                AppSettingsManager.SetSetting("Language", CurrentLanguage.ToString());
                restart = true;
            }

            AppSettingsManager.SetSetting("SectionsNumber", SectionsNumber.ToString());


            AppSettingsManager.SetSetting("Line1KKM", Line1KKM);
            AppSettingsManager.SetSetting("Line2KKM", Line2KKM);
            AppSettingsManager.SetSetting("Line3KKM", Line3KKM);
            AppSettingsManager.SetSetting("Line4KKM", Line4KKM);

            AppSettingsManager.SetSetting("Red", Red.ToString());
            AppSettingsManager.SetSetting("Orange", Orange.ToString());
            AppSettingsManager.SetSetting("Yellow", Yellow.ToString());

            AppSettingsManager.SetSetting("ShowCardNums", ShowCardNums ? "1" : "0");
            AppSettingsManager.SetSetting("ShowNames", ShowNames ? "1" : "0");
            AppSettingsManager.SetSetting("ShowPhones", ShowPhones ? "1" : "0");

            AppSettingsManager.FlushCache();


            if(CurrentDivision != Guid.Parse(AppSettingsManager.GetSetting("DivisionId")))
            {
                AppSettingsManager.SetSetting("DivisionId", CurrentDivision.ToString());
                restart = true;
            }

            if(restart)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void SyncButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //SyncButton.IsEnabled = false;
                ClientContext.DoSync();
            }
            catch(Exception ex)
            {
                ExtraWindow.Alert("Ошибка при синхронизации", ex.Message);
            }
            finally
            {
                //SyncButton.IsEnabled = true;
                LocalSettings = ClientContext.GetLocalSettings();
            }
        }

        private void UpdateKeyButton_Click(object sender, RoutedEventArgs e)
        {
            ClientContext.UpdateLicenseKey();
            LocalSettings = ClientContext.GetLocalSettings();
        }

        private void CommintNotifyButton_Click(object sender, RoutedEventArgs e)
        {
            if(LocalSettings == null) return;
            ClientContext.PostLocalSettings(LocalSettings.NotifyKeyDays, LocalSettings.NotifyKeyPeriod, LocalSettings.NotifyLicenseDays, LocalSettings.NotifyLicensePeriod, LocalSettings.NotifyAdresses);
            LocalSettings = ClientContext.GetLocalSettings();
        }

        private void ChPwdButton_Click(object sender, RoutedEventArgs e)
        {
            ClientContext.StartChangePassword("Запрос пользователя");
        }

        private void PrintActButton_Click(object sender, RoutedEventArgs e)
        {
            ActGenerator.GenerateAct(ClientContext, ReportManager);
        }
    }
}
