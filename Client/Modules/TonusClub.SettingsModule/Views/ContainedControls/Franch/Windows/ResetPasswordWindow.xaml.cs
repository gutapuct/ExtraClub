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
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;
using TonusClub.UIControls;
using TonusClub.UIControls.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows
{
    /// <summary>
    /// Interaction logic for ResetPasswordWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow
    {
        public User User { get; set; }

        public ResetPasswordWindow(ClientContext context, User user)
            : base(context)
        {
            User = user;
            InitializeComponent();
            DataContext = this;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            if (Password.Password != Password2.Password)
            {
                TonusWindow.Alert("Ошибка", "Введенные пароли не совпадают!");
                return;
            }
            if (Password.Password.Length < 4)
            {
                TonusWindow.Alert("Ошибка", "Слишком простой пароль!");
                return;
            }
            _context.ResetPassword(User.UserId, Password.Password);
            DialogResult = true;
            Close();
        }
    }
}
