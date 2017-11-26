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
using ExtraClub.UIControls.Windows;
using Telerik.Windows.Controls;
using ExtraClub.SettingsModule.Views.ContainedControls.Network.Windows;
using ExtraClub.ServiceModel;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Network
{
    public partial class AdvertSettings
    {

        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public AdvertSettings()
        {
            InitializeComponent();
        }

        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            ExtraWindow.Prompt("Новая рекламная группа",
                "Укажите название новой группы:",
                "",
                wnd => AddGroupClosed(wnd)
            );
        }

        private void AddGroupClosed(PromptWindow e)
        {
            if (e.DialogResult ?? false)
            {
                try
                {
                    if (String.IsNullOrEmpty(e.TextResult.Trim())) throw new Exception("Необходимо указать название группы");
                    Model.ClientContext.PostAdvertGroup(Guid.NewGuid(), e.TextResult.Trim());
                    Model.RefreshAdvertGroups();
                }
                catch (Exception ex)
                {
                    ExtraWindow.Alert(new DialogParameters
                    {
                        Header = "Ошибка",
                        Content = ex.Message,
                        OkButtonContent = "ОК",
                        Owner = Application.Current.MainWindow
                    });
                }
            }
        }

        private void EditGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentAdvertGroup == null) return;
            ExtraWindow.Prompt("Редактирование рекламной группы",
                "Укажите название группы:",
                Model.CurrentAdvertGroup.Name,
                wnd => EditGroupClosed(wnd));
        }

        private void EditGroupClosed(PromptWindow e)
        {
            if (Model.CurrentAdvertGroup == null) return;
            if (e.DialogResult ?? false)
            {
                try
                {
                    if (String.IsNullOrEmpty(e.TextResult.Trim())) throw new Exception("Необходимо указать название группы");
                    Model.ClientContext.PostAdvertGroup(Model.CurrentAdvertGroup.Id, e.TextResult.Trim());
                    Model.RefreshAdvertGroups();
                }
                catch (Exception ex)
                {
                    ExtraWindow.Alert(new DialogParameters
                    {
                        Header = "Ошибка",
                        Content = ex.Message,
                        OkButtonContent = "ОК",
                        Owner = Application.Current.MainWindow
                    });
                }
            }
        }

        private void RemoveGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentAdvertGroup == null) return;
            ExtraWindow.Confirm("Удаление группы", "Удалить выделенную группу? Элементы текущей группы будут удалены", e1 =>
            {
                if (e1.DialogResult ?? false)
                {
                    Model.ClientContext.DeleteAdvertGroup(Model.CurrentAdvertGroup.Id);
                    Model.RefreshAdvertGroups();
                    Model.RefreshAdvertTypes();
                }
            });

        }

    }
}
