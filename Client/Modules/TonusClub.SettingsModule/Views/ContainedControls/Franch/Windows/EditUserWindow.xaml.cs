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
using Microsoft.Practices.Unity;
using System.ServiceModel;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows
{
    /// <summary>
    /// Interaction logic for EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow
    {
        public Employee Employee { get; set; }
        public User User { get; set; }

        public List<Role> Roles { get; set; }

        IUnityContainer _container;

        public EditUserWindow(IUnityContainer container, ClientContext context, User user)
            : base(context)
        {
            _container = container;
            User = user;
            Roles = context.GetRoles();
            Roles.ForEach(i =>
            {
                if (User.SerializedRoleIds.Contains(i.RoleId))
                {
                    i.Helper = true;
                }
            });
            if (user.EmployeeId.HasValue)
            {
                Employee = context.GetEmployeeById(user.EmployeeId.Value);
            }
            InitializeComponent();
            DataContext = this;
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostUser(User.UserId, User.FullName, User.IsActive, Roles.Where(i => i.Helper).Select(i => i.RoleId).ToArray(), User.Email);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ResetPasswordButtonClick(object sender, RoutedEventArgs e)
        {
            ModuleViewBase.ProcessUserDialog<ResetPasswordWindow>(_container, ()=>{}, new ResolverOverride[] { new ParameterOverride("user", User)});
        }
    }
}
