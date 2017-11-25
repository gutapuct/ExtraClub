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
using TonusClub.Infrastructure.Interfaces;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using TonusClub.OrganizerModule.ViewModels;
using TonusClub.ServiceModel;
using TonusClub.UIControls;
using Microsoft.Practices.Unity;
using TonusClub.OrganizerModule.Views.Windows;
using TonusClub.Infrastructure.Events;
using System.ComponentModel;
using TonusClub.OrganizerModule.Views.Tasks.Windows;
using System.Timers;

namespace TonusClub.OrganizerModule.Views
{
    public partial class OrganizerLargeView : ModuleViewBase, ILargeSection
    {
        private OrganizerLargeViewModel Model { get; set; }

        public OrganizerLargeView(OrganizerLargeViewModel model)
        {
            InitializeComponent();
            Model = model;

            DataContext = Model;

            tooltipLoader.DoWork += new DoWorkEventHandler(tooltipLoader_DoWork);
            tooltipLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(tooltipLoader_RunWorkerCompleted);

            NavigationManager.NewCustomerNotificationRequest += NavigationManager_DoNewCustomerNotification;

            NavigationManager.CustomerNotificationRequest += NavigationManager_CustomerNotificationRequest;

            var t = new Timer(600000);
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            t.Start();
        }

        void NavigationManager_CustomerNotificationRequest(object sender, GuidEventArgs e)
        {
            var cn = ClientContext.GetCustomerNotificationById(e.Guid);
            ProcessUserDialog<CustomerNotificationTaskWindow>(() =>
            {
                e.OnClose();
                Model.RefreshTasks();
                Model.RefreshArchivedTasks();
            }, new ResolverOverride[] { new ParameterOverride("item", cn) });
        }


        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
                            tooltipLoader.RunWorkerAsync(new { Context = ClientContext, Parameter = new object() })));
        }

        void NavigationManager_DoNewCustomerNotification(object sender, ClientEventArgs e)
        {
            ProcessUserDialog<NewCallsTask>(() => e.OnSuccess(Guid.Empty), new ResolverOverride[] { new ParameterOverride("customers", new Guid[] { e.ClientId }) });
        }

        static DateTime LastCheckTime = DateTime.Now;

        void tooltipLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = ((dynamic)e.Argument).Context.GetFromSite(LastCheckTime);
            if(e.Result == null)
            {
                e.Result = (((dynamic)e.Argument).Context.GetNotificationsForUser()) as string + (((dynamic)e.Argument).Context.GetNotificationsForDivision()) as string;
                if(string.Empty.Equals(e.Result) && ((dynamic)e.Argument).Parameter != null) e.Result = null;
            }
        }

        void tooltipLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Result == null) return;
                string title = "Напоминания";
                string text = (string)e.Result;
                if (String.IsNullOrEmpty(text)) text = "Нет напоминаний";

                MyNotifyIcon.ShowBalloonTip(title, text, MyNotifyIcon.Icon);
            }
            catch
            {
                ;
            }
        }

        public object GetTransferDataForMinimize()
        {
            return null;
        }

        public object GetTransferDataForRestore()
        {
            return null;
        }

        public override void SetState(object data)
        {
            base.SetState(data);
            Model.EnsureDataLoading();
        }

        BackgroundWorker tooltipLoader = new BackgroundWorker();

        private void MyNotifyIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            tooltipLoader.RunWorkerAsync(new { Context = ClientContext, Parameter = (object)null });
        }
    }
}
