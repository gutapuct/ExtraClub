using System.Windows;
using System.Windows.Input;
using ExtraClub.Infrastructure;

namespace ExtraClub.ClientDal
{
    public partial class LoginWindow : Window
    {
        public string MainInstruction { get; set; }
        public string UserName { get; set; }
        public string Password
        {
            get
            {
                return PasswordBox.Password;
            }
        }

        public bool IsSave { get; set; }

        public LoginWindow(string mainInstruction)
        {
            MainInstruction = mainInstruction;
            Mouse.OverrideCursor = null;
            IsSave = AppSettingsManager.GetSetting("SaveCreds") == "1";
            if (IsSave)
            {
                UserName = AppSettingsManager.GetSetting("Username");
            }
            DataContext = this;
            InitializeComponent();
            if (IsSave)
            {
                PasswordBox.Password = AppSettingsManager.GetSetting("Password");
            }

        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        internal void ConfirmCredentials()
        {
            AppSettingsManager.SetSetting("SaveCreds", IsSave ? "1" : "0");
            if (IsSave)
            {
                AppSettingsManager.SetSetting("Username", UserName);
                AppSettingsManager.SetSetting("Password", Password);
            }
            else
            {
                AppSettingsManager.SetSetting("Username", "");
                AppSettingsManager.SetSetting("Password", "");
            }
        }
    }
}
