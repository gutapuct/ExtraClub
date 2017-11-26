using Telerik.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using FlagmaxControls.BaseClasses;
using ExtraClub.Infrastructure;
using Microsoft.Practices.Unity;
using System;

namespace ExtraClub.UIControls
{
    public class WindowBase : DialogBase, INotifyPropertyChanged
    {
        public ResizeMode ResizeMode { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public WindowStartupLocation WindowStartupLocation { get; set; }
        public WindowState WindowState { get; set; }
        public object Owner { get; set; }

        protected ClientContext _context { get; private set; }

        public WindowBase()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _context = ApplicationDispatcher.UnityContainer.Resolve<ClientContext>();
            }
        }

        [Obsolete("Не надо сюда ничего передавать!")]
        public WindowBase(ClientContext context) : this() { }

        public override bool? ShowDialog()
        {
            AuthorizationManager.ApplyPermissions(this.ParentOfType<Window>());

            return base.ShowDialog();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
