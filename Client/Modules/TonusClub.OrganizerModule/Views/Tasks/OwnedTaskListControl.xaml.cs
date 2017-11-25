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
using TonusClub.OrganizerModule.Views.Tasks.Windows;
using Microsoft.Practices.Unity;
using TonusClub.OrganizerModule.ViewModels;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Windows;
using TonusClub.UIControls;

namespace TonusClub.OrganizerModule.Views.Tasks
{
    /// <summary>
    /// Interaction logic for OwnedTaskListControl.xaml
    /// </summary>
    public partial class OwnedTaskListControl
    {
        private OrganizerLargeViewModel Model
        {
            get
            {
                return DataContext as OrganizerLargeViewModel;
            }
        }

        public OwnedTaskListControl()
        {
            InitializeComponent();
#if BEAUTINIKA
            closebtn.Content = "Закрытие студии";
#endif
            NavigationManager.CallTaskRequest += NavigationManager_CallTaskRequest;
        }

        void NavigationManager_CallTaskRequest(object sender, ObjectEventArgs e)
        {
            ProcessUserDialog<NewCallsTask>(() =>
            {
                Model.RefreshCalls();
                Model.RefreshTasks();
                Model.RefreshMyTasks();
            }, new ParameterOverride("customers", e.Object));
        }

        private void OutboxTasksGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                if (Model.OutboxOrganizerView.CurrentItem != null)
                {
                    var i = Model.OutboxOrganizerView.CurrentItem as OrganizerItem;
                    TonusWindow.Alert("Задача", String.IsNullOrEmpty(i.CompletionComment) ? "Комментарий не указан" : i.CompletionComment);
                }
            }
        }

        private void ClubClosingButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<ClubClosingWindow>(() =>
            {
                Model.RefreshTasks();
                Model.RefreshMyTasks();
            });
        }

        private void NewCallsButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewCallsTask>(() =>
            {
                Model.RefreshCalls();
                Model.RefreshTasks();
                Model.RefreshMyTasks();
            });
        }

        private void NewTaskButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewTaskWindow>(() =>
            {
                Model.RefreshTasks();
                Model.RefreshMyTasks();
            });
        }

        private void RecallButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.OutboxOrganizerView.CurrentItem == null) return;
            var i = Model.OutboxOrganizerView.CurrentItem as OrganizerItem;
            if (i.Data == null)
            {
                TonusWindow.Alert("Ошибка", "Невозможно отозвать выделенную задачу, т.к. она является частью другой задачи!");
                return;
            }
            TonusWindow.Confirm("Отзыв задачи", "Вы действительно хотите отозвать задачу?", w =>
            {
                if (w.DialogResult ?? false)
                {
                    ClientContext.PostTaskRecall((Guid)i.Data);
                    Model.RefreshTasks();
                    Model.RefreshMyTasks();
                    Model.RefreshArchivedTasks();
                }
            });
        }
    }
}
