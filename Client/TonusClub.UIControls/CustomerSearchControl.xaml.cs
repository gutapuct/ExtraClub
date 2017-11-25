using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Windows;
using TonusClub.Infrastructure;

namespace TonusClub.UIControls
{
    public partial class CustomerSearchControl : UserControl, INotifyPropertyChanged
    {

        BackgroundWorker searchWorker = new BackgroundWorker();

        public ClientContext ClientContext
        {
            get
            {
                return UnityContainer.Resolve<ClientContext>();
            }
        }

        public bool AllowInput
        {
            get { return (bool)GetValue(AllowInputProperty); }
            set
            {
                SetValue(AllowInputProperty, value);
                CriteriaCombo.IsEditable = value;
                OnPropertyChanged("AllowInput");
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowInputProperty =
            DependencyProperty.Register(
                "AllowInput",
                typeof(bool),
                typeof(CustomerSearchControl),
                new System.Windows.PropertyMetadata(true));



        public CustomerSearchControl()
        {
            InitializeComponent();

            CriteriaCombo.IsEditable = AllowInput;
            CustomersResultView = CollectionViewSource.GetDefaultView(CustomersResult);

            DataContext = this;

            searchWorker.WorkerSupportsCancellation = true;
            searchWorker.DoWork += new DoWorkEventHandler(StartSearch);
            searchWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SearchFinished);

            parsecReader = new ParsecReader(0);
            parsecReader.CardChanged += new EventHandler<CardEventArgs>(parsecReader_CardChanged);

        }

        void parsecReader_CardChanged(object sender, CardEventArgs e)
        {
            SelectByCardNumber(e.CardNumber);
            IsListening = false;
        }

        public void CollapseDropDown()
        {
            CriteriaCombo.IsDropDownOpen = false;
        }

        public event EventHandler<GuidEventArgs> SelectedClientChanged;

        public class CustomerEventArgs : EventArgs
        {
            public Customer Customer { get; set; }
        }

        private void CriteriaCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedClientChanged != null && e.AddedItems.Count > 0)
            {
                SelectedClientChanged(this, new GuidEventArgs { Guid = ((FoundCustomer)e.AddedItems[0]).Id });
            }
        }

        public void SelectByCardNumber(int cardNumber)
        {
            var customer = ClientContext.GetCustomerByCard(cardNumber, false);
            if (customer == null) return;
            CustomersResult.Clear();
            CustomersResult.Add(new FoundCustomer { Id = customer.Id, CardNumber = cardNumber.ToString(), FullName = customer.FullName });
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CustomersResultView.Refresh();
                CriteriaCombo.SelectedIndex = 0;
            }), new object[0]);
        }


        public void SelectById(Guid id)
        {
            var customer = ClientContext.GetCustomer(id);
            if (customer == null) return;
            CustomersResult.Clear();
            CustomersResult.Add(new FoundCustomer { Id = customer.Id, CardNumber = (customer.ActiveCard != null ? customer.ActiveCard.CardBarcode : ""), FullName = customer.FullName });
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CustomersResultView.Refresh();
                CriteriaCombo.SelectedIndex = 0;
            }), new object[0]);
        }

        private void CriteriaText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && AllowInput) ProcessSearch();
        }

        void StartSearch(object sender, DoWorkEventArgs e)
        {
            ModelSearchCustomers((string)e.Argument);
        }

        private void ProcessSearch()
        {
            this.IsEnabled = false;
            if (searchWorker.IsBusy) return;
            while (searchWorker.CancellationPending) { }
            searchWorker.RunWorkerAsync(CriteriaCombo.Text);
        }

        void SearchFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) return;

            CustomersResultView.Refresh();
            this.IsEnabled = true;
            CriteriaCombo.Focus();

            if (!CustomersResultView.IsEmpty)
            {
                CriteriaCombo.IsDropDownOpen = true;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessSearch();
        }

        #region Model

        private List<FoundCustomer> CustomersResult = new List<FoundCustomer>();
        public ICollectionView CustomersResultView { get; private set; }

        internal void ModelSearchCustomers(string searchKey)
        {
            CustomersResult.Clear();
            CustomersResult.AddRange(ClientContext.SearchCustomers(searchKey));
        }

        bool _isListening;

        public bool IsListening
        {
            get
            {
                return _isListening;
            }
            set
            {
                if (IsListening != value)
                {
                    OnPropertyChanged("IsListening");
                }
                _isListening = value;

                if (IsListening)
                {
                    try
                    {
                        parsecReader.StartListening();
                    }
                    catch (Exception)
                    {
                        TonusWindow.Prompt(UIControls.Localization.Resources.ManualInput,
                             UIControls.Localization.Resources.ProvideCardNumber,
                             "",
                            wnd => EditClosed(wnd));
                    }
                }
                else
                {
                    parsecReader.StopListening();
                }
            }
        }

        private void EditClosed(PromptWindow wnd)
        {
            if (wnd.DialogResult ?? false)
            {
                var s = (wnd.TextResult ?? "").Trim();
                int i;
                if (Int32.TryParse(s, out i))
                {
                    parsecReader_CardChanged(null, new CardEventArgs { CardNumber = i });
                }
            }
            parsecReader.StopListening();
        }
        #endregion

        ParsecReader parsecReader;


        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public IUnityContainer UnityContainer => ApplicationDispatcher.UnityContainer;
    }
}
