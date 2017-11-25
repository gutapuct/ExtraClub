using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls;
using System.Windows.Controls;
using System.Windows;
using TonusClub.ServiceModel.Reports;
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.Reports.Views.ContainedControls
{
    class TabReportItem : TabItem
    {
        public string ReportKey { get; set; }

        public TabReportItem(IUnityContainer container, ReportInfoInt report, object[] parameters)
        {
            ReportKey = report.Key;
            if (parameters.Length ==1 && report.Key=="GetTicketRemainReport")
            {
                report.Parameters[0].InstanceValue = container.Resolve<ClientContext>().CurrentDivision.Id;
                report.Parameters[1].InstanceValue = parameters[0];
            }
            if (parameters!=null && report.Key != "GetTicketRemainReport")
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (report.Parameters.Count > i)
                    {
                        report.Parameters[i].InstanceValue = parameters[i];
                    }
                }
            }
            var sp = new StackPanel {Orientation = System.Windows.Controls.Orientation.Horizontal};
            sp.Children.Add(new TextBlock
            {
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Text = report.Name,
                Margin = new System.Windows.Thickness(0, 0, 5, 0)
            });
            var btn = new Button
            {
                Content = "Закрыть",
                Padding = new System.Windows.Thickness(5, 0, 5, 0)
            };
            btn.Click += new System.Windows.RoutedEventHandler(CloseTabButton_Click);
            sp.Children.Add(btn);
            Header = sp;

            Content = ReportContainerBase.CreateInstance(report);
            if (parameters.Length > 0)
            {
                Dispatcher.BeginInvoke(new Action(() => (Content as ReportContainerBase).GenerateClick(null, null)));
            }
            AuthorizationManager.ApplyPermissions(null);
        }

        private void CloseTabButton_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as FrameworkElement;
            var tab = s.ParentOfType<TabItem>();
            if (tab != null)
            {
                tab.ParentOfType<TabControl>().Items.Remove(tab);
            }
        }
    }
}
