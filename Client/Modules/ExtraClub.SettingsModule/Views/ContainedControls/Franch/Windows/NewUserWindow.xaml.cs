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
using ExtraClub.UIControls.Windows;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;
using System.ServiceModel;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Franch.Windows
{
    /// <summary>
    /// Interaction logic for NewUserWindow.xaml
    /// </summary>
    public partial class NewUserWindow
    {
        public Guid Result { get; set; }

        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Employee Employee { get; set; }
        public string UserPrefix { get; set; }

        public NewUserWindow(ClientContext context, Guid employeeId)
        {
            Employee = context.GetEmployeeById(employeeId);
            UserName = (Employee.SerializedCustomer.FirstName ?? "_")[0].ToString().ToLatin() + Employee.SerializedCustomer.LastName.ToLatin();
            FullName = Employee.SerializedCustomer.ShortName;
            UserPrefix = context.CurrentCompany.UserPrefix;
            InitializeComponent();
            DataContext = this;
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            if (Password.Password != Password2.Password)
            {
                ExtraWindow.Alert("Ошибка", "Введенные пароли не совпадают!");
                return;
            }
            if (Password.Password.Length < 4)
            {
                ExtraWindow.Alert("Ошибка", "Слишком простой пароль!");
                return;
            }
            if (UserName.Length < 4)
            {
                ExtraWindow.Alert("Ошибка", "Слишком короткое имя пользователя!");
                return;
            }
            try
            {
                Result = _context.PostNewUser(Employee.Id, UserName, FullName, Password.Password, Email);
            }
            catch (FaultException ex)
            {
                ExtraWindow.Alert("Ошибка", ex.Message);
                return;
            }
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
