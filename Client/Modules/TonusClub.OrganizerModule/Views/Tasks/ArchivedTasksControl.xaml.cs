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
using TonusClub.OrganizerModule.ViewModels;
using TonusClub.UIControls.Windows;
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.OrganizerModule.Views.Tasks
{
    public partial class ArchivedTasksControl
    {
        private OrganizerLargeViewModel Model
        {
            get
            {
                return DataContext as OrganizerLargeViewModel;
            }
        }

        public ArchivedTasksControl()
        {
            InitializeComponent();
        }

        private void ReopenButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.ArchivedOrganizerView.CurrentItem == null) return;
            var i = Model.ArchivedOrganizerView.CurrentItem as OrganizerItem;

            if (i.Data == null)
            {
                TonusWindow.Alert("Ошибка", "Невозможно возобновить выделенную задачу, т.к. она является частью другой задачи!");
                return;
            }
            TonusWindow.Confirm("Возобновление", "Вы действительно хотите возобновить выделенную задачу?", wnd =>
            {
                if (wnd.DialogResult ?? false)
                {
                    ClientContext.PostTaskReopen((Guid)i.Data);
                    Model.RefreshTasks();
                    Model.RefreshMyTasks();
                    Model.RefreshArchivedTasks();
                }
            });
        }

        private void ArchivedTasksGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                if (Model.ArchivedOrganizerView.CurrentItem != null)
                {
                    var i = Model.ArchivedOrganizerView.CurrentItem as OrganizerItem;
                    TonusWindow.Alert("Комментарий о завершении задачи", String.IsNullOrEmpty(i.CompletionComment) ? "Комментарий не указан" : i.CompletionComment);
                }
            }
        }

        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.ArchivedOrganizerView.CurrentItem == null) return;
            if (Model.ArchivedOrganizerView.CurrentItem is OrganizerItem && ((OrganizerItem)Model.ArchivedOrganizerView.CurrentItem).CustomerId.HasValue)
            {
                NavigationManager.MakeClientRequest(((OrganizerItem)Model.ArchivedOrganizerView.CurrentItem).CustomerId.Value);
            }
        }
    }
}
