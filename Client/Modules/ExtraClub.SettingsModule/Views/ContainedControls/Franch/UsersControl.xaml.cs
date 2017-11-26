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
using ExtraClub.SettingsModule.ViewModels;
using ExtraClub.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.Windows;
using ExtraClub.SettingsModule.Views.ContainedControls.Franch.Windows;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.UIControls;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Franch
{
    /// <summary>
    /// Interaction logic for UsersControl.xaml
    /// </summary>
    public partial class UsersControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public UsersControl()
        {
            InitializeComponent();
            NavigationManager.ActivateLoginsRequest += new EventHandler<ObjectEventArgs>(UsersControl_DoActivateLogins);
        }

        void UsersControl_DoActivateLogins(object sender, ObjectEventArgs e)
        {
            Model.WaitUpdateFinished();
            //detect if employee already has a registered user.
            var user = Model._UsersView.FirstOrDefault(i => i.EmployeeId.HasValue && i.EmployeeId.Value == (Guid)e.Object);
            if (user == null)
            {
                //if not, offer to create new user.
                ExtraWindow.Confirm("Доступ", "Сотрудник не имеет доступа к АСУ.\nСоздать для него учетную запись?", e1 =>
                {
                    if (e1.DialogResult ?? false)
                    {

                        ProcessUserDialog<NewUserWindow>(() =>
                        {
                            Model.RefreshUsers();
                            user = Model._UsersView.FirstOrDefault(i => i.EmployeeId.HasValue && i.EmployeeId.Value == (Guid)e.Object);
                            Model.UsersView.MoveCurrentTo(user);
                            EditButton_Click(null, null);
                        }, new ParameterOverride("employeeId", (Guid)e.Object));
                    }
                });
            }
            else
            {
                Model.UsersView.MoveCurrentTo(user);
                EditButton_Click(null, null);
            }
        }

        private void UsersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("FranchUsersMgmt"))
            {
                EditButton_Click(null, null);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.UsersView.CurrentItem == null) return;
            ProcessUserDialog<EditUserWindow>(() => Model.RefreshUsers(), new ParameterOverride("user", ViewModelBase.Clone<User>(Model.UsersView.CurrentItem)));
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.UsersView.CurrentItem == null) return;
            NavigationManager.MakeUserActionsReportRequest(((User)Model.UsersView.CurrentItem).UserName);
        }
    }
}
