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
using System.ComponentModel;
using TonusClub.Infrastructure.BaseClasses;
using Microsoft.Practices.Unity;
using TonusClub.UIControls.BaseClasses;

namespace TonusClub.SettingsModule.Views.ContainedControls.Network.Windows
{
    /// <summary>
    /// Interaction logic for NewEditCallScrenarioFormWindow.xaml
    /// </summary>
    public partial class NewEditCallScrenarioFormWindow
    {
        public IncomingCallForm IncomingCallForm { get; set; }

        public ICollectionView ButtonsView { get; set; }

        IUnityContainer _container;

        public NewEditCallScrenarioFormWindow(ClientContext context, IncomingCallForm form, IUnityContainer container)
            : base(context)
        {
            if (form.SerializedIncomingCallFormButtons == null) form.SerializedIncomingCallFormButtons = new List<IncomingCallFormButton>();
            _container = container;
            IncomingCallForm = form;
            ButtonsView = CollectionViewSource.GetDefaultView(IncomingCallForm.SerializedIncomingCallFormButtons);
            DataContext = this;
            InitializeComponent();
        }

        private void RadGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ModuleViewBase.IsRowClicked(e) && ButtonsView.CurrentItem != null)
            {
                var btn = ViewModelBase.Clone<IncomingCallFormButton>(ButtonsView.CurrentItem);
                ModuleViewBase.ProcessUserDialog<NewEditButtonWindow>(_container, dlg =>
                {
                    if (dlg.DialogResult ?? false == true)
                    {
                        IncomingCallForm.SerializedIncomingCallFormButtons.Remove((IncomingCallFormButton)ButtonsView.CurrentItem);
                        IncomingCallForm.SerializedIncomingCallFormButtons.Add(dlg.ButtonResult);
                        ButtonsView.Refresh();
                    }
                }, new ParameterOverride("button", btn));

            }
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostIncomingCallForm(IncomingCallForm);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            ModuleViewBase.ProcessUserDialog<NewEditButtonWindow>(_container, dlg =>
            {
                IncomingCallForm.SerializedIncomingCallFormButtons.Add(dlg.ButtonResult);
                ButtonsView.Refresh();
            });
        }

        private void RemoveButtonClick(object sender, RoutedEventArgs e)
        {
            if (ButtonsView.CurrentItem == null) return;
            IncomingCallForm.SerializedIncomingCallFormButtons.Remove((IncomingCallFormButton)ButtonsView.CurrentItem);
            ButtonsView.Refresh();
        }
    }
}
