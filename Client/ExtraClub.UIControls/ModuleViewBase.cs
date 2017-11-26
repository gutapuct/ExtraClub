using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using Microsoft.Practices.Unity;
using ExtraClub.Infrastructure;
using ExtraClub.Infrastructure.Interfaces;
using System.ComponentModel;
using System.Windows.Media;
using ExtraClub.UIControls.Interfaces;

namespace ExtraClub.UIControls
{
    public class ModuleViewBase : UserControl, INotifyPropertyChanged
    {
        protected virtual void OnInitializing()
        {
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected ISettingsManager SettingsManager
        {
            get
            {
                return ApplicationDispatcher.UnityContainer.Resolve<ISettingsManager>();
            }
        }

        protected ClientContext ClientContext { get; }


        protected IReportManager ReportManager => ApplicationDispatcher.UnityContainer.Resolve<IReportManager>();

        protected IDictionaryManager DictionaryManager
        {
            get
            {
                return ApplicationDispatcher.UnityContainer.Resolve<IDictionaryManager>();
            }
        }

        public ModuleViewBase()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                ClientContext = ApplicationDispatcher.UnityContainer.Resolve<ClientContext>();
            }
        }

        public static bool IsRowClicked(MouseButtonEventArgs e)
        {
            FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
            if (originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                return row != null;
            }
            return false;
        }

        public static GridViewRow RowClicked(MouseButtonEventArgs e)
        {
            FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
            if (originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                return row;
            }
            return null;
        }

        public void ProcessUserDialog<T>(Action closedOk)
            where T : WindowBase
        {
            ProcessUserDialog<T>(ApplicationDispatcher.UnityContainer, closedOk, new ParameterOverride("customers", new Guid[0]));
        }

        public void ProcessUserDialog<T>(Action<T> closed)
            where T : WindowBase
        {
            ProcessUserDialog<T>(ApplicationDispatcher.UnityContainer, closed);
        }

        public void ProcessUserDialog<T>(Action closedOk, params ResolverOverride[] overrides)
            where T : WindowBase
        {
            ProcessUserDialog<T>(ApplicationDispatcher.UnityContainer, closedOk, overrides);
        }

        public void ProcessUserDialog<T>(Action<T> closed, params ResolverOverride[] overrides)
            where T : WindowBase
        {
            ProcessUserDialog<T>(ApplicationDispatcher.UnityContainer, closed, overrides);
        }

        public void ProcessUserDialog<T>(Action<T> closed, Action<T> initialize, params ResolverOverride[] overrides)
            where T : WindowBase
        {
            var wnd = ApplicationDispatcher.UnityContainer.Resolve<T>(overrides);
            initialize(wnd);
            wnd.Closed = () => { closed(wnd); };
            wnd.ShowDialog();
        }


        public static void ProcessUserDialog<T>(IUnityContainer container, Action closedOk, params ResolverOverride[] overrides)
            where T : WindowBase
        {
            var wnd = container.Resolve<T>(overrides);
            wnd.Closed = () => { if (wnd.DialogResult ?? false) closedOk(); };
            wnd.ShowDialog();
        }

        public static void ProcessUserDialog<T>(IUnityContainer container, Action<T> closed, params ResolverOverride[] overrides)
            where T : WindowBase
        {
            var wnd = container.Resolve<T>(overrides);
            wnd.Closed = () => { closed(wnd); };
            wnd.ShowDialog();
        }

        public static void ProcessUserDialog<T>(IUnityContainer container, Action<T> closed, Action<T> initialize)
            where T : WindowBase
        {
            var wnd = container.Resolve<T>();
            initialize(wnd);
            wnd.Closed = () => { closed(wnd); };
            wnd.ShowDialog();
        }

        public virtual void SetState(object data)
        {
            foreach (RadGridView grid in FindVisualChildren<RadGridView>(this))
            {
                grid.UpdateLayout();
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
