using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using TonusClub.ServiceModel;
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure;
using TonusClub.UIControls.Windows;

namespace TonusClub.UIControls
{
    /// <summary>
    /// Interaction logic for ProviderSelector.xaml
    /// </summary>
    public partial class ProviderSelector : UserControl, INotifyPropertyChanged
    {
        public ProviderSelector()
        {
            InitializeComponent();
        }

        public Guid SelectedId
        {
            get { return (Guid)GetValue(SelectedIdProperty); }
            set
            {
                SetValue(SelectedIdProperty, value);
                OnPropertyChanged("SelectedId");
                if (Providers != null)
                {
                    var p = Providers.SingleOrDefault(i => i.Id == SelectedId);
                    if (p != null)
                    {
                        ProviderName.Text = p.Name;
                    }
                }
            }
        }

        public static readonly DependencyProperty SelectedIdProperty =
           DependencyProperty.Register(
               "SelectedId",
               typeof(Guid),
               typeof(ProviderSelector),
               new System.Windows.PropertyMetadata(Guid.Empty));

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        ClientContext _context;
        private ClientContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = ApplicationDispatcher.UnityContainer.Resolve<ClientContext>();
                }
                return _context;
            }
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new ProviderSelectorWindow(Folders, Providers);
            wnd.Closed = () => {
                if (wnd.DialogResult ?? false)
                {
                    if (wnd.SelectedId.HasValue)
                    {
                        SelectedId = wnd.SelectedId.Value;
                    }
                }
            };
            wnd.ShowDialog();
        }

        private List<Provider> _providers;
        private List<Provider> Providers
        {
            get
            {
                if (_providers == null)
                {
                    if (Context != null)
                    {
                        _providers = Context.GetAllProviders();
                    }
                }
                return _providers;
            }
        }

        private List<ProviderFolder> _folders;
        private List<ProviderFolder> Folders
        {
            get
            {
                if (_folders == null)
                {
                    if (Context != null)
                    {
                        _folders = Context.GetProviderFolders();
                    }
                }
                return _folders;
            }
        }    }

}
