using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using ExtraClub.CashRegisterModule;
using ExtraClub.ServiceModel;
using System.ComponentModel;
using ExtraClub.Infrastructure.Interfaces;
using System.Windows.Data;
using System.Data;
using System.Configuration;
using ExtraClub.Infrastructure.BaseClasses;
using Telerik.Windows.Controls;
using System.Windows;
using Microsoft.Practices.Unity;
using ExtraClub.UIControls.Windows;
using ExtraClub.UIControls;
using ViewModelBase = ExtraClub.UIControls.BaseClasses.ViewModelBase;

namespace ExtraClub.TurnoverModule.ViewModels
{
    public class BarPointViewModel : ViewModelBase
    {
        private IList<Customer> PresentCustomers { get; set; }
        private List<BarPointGood> DivisionGoods { get; set; }
        public ICollectionView PresentCustomersView { get; set; }
        public ICollectionView DivisionGoodsView { get; private set; }
        private IList<GoodAction> CurrentActions { get; set; }

        public decimal DiscountPercent { get; set; }

        private Customer _currentCustomer;
        public Customer CurrentCustomer
        {
            get
            {
                return _currentCustomer;
            }
            set
            {
                _currentCustomer = value;
                if (value != null)
                {
                    DiscountPercent = ClientContext.GetBarDiscountForCustomer(value.Id);
                }
                else
                {
                    DiscountPercent = 0;
                }
                OnPropertyChanged("CurrentCustomer");
                OnPropertyChanged("DiscountPercent");
            }
        }

        //private string _totalPositios;
        public string TotalPositions
        {
            get
            {
                return GetAmountOfPositionsInBasket().ToString();
            }
        }

        //private string _totalItems;
        public string TotalItems
        {
            get
            {
                return GetAmountOfItemsInBasket().ToString();
            }
        }


        public string TotalAmount
        {
            get
            {
                var res = GetCostOfPositionsInBasket().ToString("c");
                if (GetBonusVisibility())
                {
                    res += " / " + GetCostOfPositionsInBasket(true).ToString("n0");
                }
                return res;
            }
        }

        private GoodAction _matchedAction;
        public GoodAction MatchedAction
        {
            get
            {
                return _matchedAction;
            }
            set
            {
                _matchedAction = value;
                OnPropertyChanged("MatchedAction");
                OnPropertyChanged("TotalAmount");
            }
        }

        public string ActionText { get; set; }

        public bool IsRefreshing { get; private set; }

        private CashRegisterManager _cashRegister;

        public BarPointViewModel(IUnityContainer container, CashRegisterManager cashRegister)
            : base()
        {
            CultureHelper.FixupCulture();

            PresentCustomers = new List<Customer>();
            PresentCustomersView = CollectionViewSource.GetDefaultView(PresentCustomers);

            DivisionGoods = new List<BarPointGood>();
            DivisionGoodsView = CollectionViewSource.GetDefaultView(DivisionGoods);

            DivisionGoodsView.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            DivisionGoodsView.SortDescriptions.Add(new SortDescription("Category", ListSortDirection.Ascending));
            DivisionGoodsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            PresentCustomersView.CurrentChanged += new EventHandler(PresentCustomersView_CurrentChanged);

            _cashRegister = cashRegister;
        }

        void PresentCustomersView_CurrentChanged(object sender, EventArgs e)
        {
            ;
        }

        protected override void RefreshDataInternal()
        {
            CultureHelper.FixupCulture();

            IsRefreshing = true;
            PresentCustomers.Clear();
            ClientContext.GetPresentCustomers().ForEach(p => { PresentCustomers.Add(p); });

            CurrentActions = ClientContext.GetGoodActions(true);

            var stores = ClientContext.GetStorehouses().Where(i => i.BarSale).Select(i => i.Id).ToList();

            DivisionGoods.Clear();
            List<BarPointGood> src;
            if (_turnoverModel != null)
            {
                src = _turnoverModel.GoodPresence;
                _turnoverModel = null;
            }
            else
            {
                src = ClientContext.GetGoodsPresence();
            }
            src.Where(i => stores.Contains(i.StorehouseId)).ToList().ForEach(p =>
                {
                    if (p.IsInPricelist)
                    {
                        DivisionGoods.Add(p);
                        //p.BackColor = "Transparent";
                        p.ClearVisible = "Hidden";
                    }
                });
            DivisionGoods.Sort();
            IsRefreshing = false;
        }

        protected override void RefreshFinished()
        {
            base.RefreshFinished();
            PresentCustomersView.Refresh();
            DivisionGoodsView.Refresh();
        }

        internal void AddToBasket(object itemObject, int amount)
        {
            if (itemObject == null) return;
            var good = (BarPointGood)itemObject;
            if (good.Amount >= amount)
            {
                good.InBasket += amount;
                good.Amount -= amount;
                //good.BackColor = "#35FFFFFF";
                good.ClearVisible = "Visible";
            }
            if (good.InBasket <= 0)
            {
                good.Amount += good.InBasket;
                good.InBasket = 0;
                //good.BackColor = "Transparent";
                good.ClearVisible = "Hidden";
            }
        }

        internal bool IsBasketEmpty()
        {
            return !DivisionGoods.Any(g => g.InBasket > 0);
        }

        internal void ClearBasket()
        {
            DivisionGoods.ToList().ForEach(g =>
            {
                AddToBasket(g, -g.InBasket);
            });
            OnPropertyChanged("TotalAmount");
            OnPropertyChanged("TotalItems");
            OnPropertyChanged("TotalPositions");
        }

        internal int GetAmountOfPositionsInBasket()
        {
            return DivisionGoods.Count(g => g.InBasket > 0);
        }

        internal decimal GetCostOfPositionsInBasket(bool isBonus = false)
        {
            decimal res = 0;
            bool isEmpl = false;
            if (CurrentCustomer != null && CurrentCustomer.IsEmployee) isEmpl = true;
            if (isBonus)
            {
                DivisionGoods.ToList().ForEach(g => res += (decimal)(g.InBasket * (g.BonusPrice ?? 0)));
            }
            else if (isEmpl)
            {
                DivisionGoods.ToList().ForEach(g => res += (decimal)(g.InBasket * g.EmployeePrice));
            }
            else
            {
                DivisionGoods.ToList().ForEach(g => res += (decimal)(g.InBasket * g.Price));
                if (MatchedAction != null)
                {
                    res *= (1 - MatchedAction.Discount);
                }
            }
            if (CurrentCustomer != null && CurrentCustomer.ActiveCard != null && CurrentCustomer.ActiveCard.SerializedCustomerCardType != null && CurrentCustomer.ActiveCard.SerializedCustomerCardType.DiscountBar > 0)
            {
                res *= (1 - CurrentCustomer.ActiveCard.SerializedCustomerCardType.DiscountBar);
            }
            return res * (1 - DiscountPercent / 100);
        }

        internal double GetAmountOfItemsInBasket()
        {
            return DivisionGoods.ToList().Sum(g => g.InBasket);
        }

        internal void ProcessPayment(bool isBonusPmt, Action<PaymentDetails> onFinish, bool isCashless = false)
        {
            if (CurrentCustomer == null)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.UnableToProcess,
                    Content = UIControls.Localization.Resources.NoCustomerSelected,
                    OkButtonContent = UIControls.Localization.Resources.Ok,
                    Owner = Application.Current.MainWindow
                });
                return;
            }

            if (GetAmountOfPositionsInBasket() == 0)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.UnableToProcess,
                    Content = UIControls.Localization.Resources.NoGoodsSelected,
                    OkButtonContent = UIControls.Localization.Resources.Ok,
                    Owner = Application.Current.MainWindow
                });
                return;
            }

            var pmt = new PaymentDetails(CurrentCustomer.Id, 0, isCashless);

            if (isBonusPmt)
            {
                pmt.RequestedBonusAmount = GetCostOfPositionsInBasket(true);
            }
            else
            {
                pmt.RequestedAmount = GetCostOfPositionsInBasket();
            }
            var pmtContent = new List<BarPointGood>();
            foreach (var g in DivisionGoods.Where(p => p.InBasket != 0))
            {
                var newGood = new BarPointGood
                {
                    GoodId = g.GoodId,
                    InBasket = g.InBasket,
                    BonusPrice = g.BonusPrice,
                    Price = g.Price,
                    Name = g.Name,
                    UnitName = g.UnitName,
                    StorehouseId = g.StorehouseId
                };
                pmtContent.Add(newGood);
                if (MatchedAction != null)
                {
                    newGood.Price = (1 - (decimal)MatchedAction.Discount) * newGood.Price;
                }
                if (CurrentCustomer.IsEmployee)
                {
                    newGood.Price = g.EmployeePrice;
                }
                newGood.Price *= 1 - DiscountPercent / 100;
            }
            if (CurrentCustomer != null && CurrentCustomer.ActiveCard != null && CurrentCustomer.ActiveCard.SerializedCustomerCardType != null && CurrentCustomer.ActiveCard.SerializedCustomerCardType.DiscountBar > 0)
            {
                pmtContent.ForEach(i => i.Price *= (1 - CurrentCustomer.ActiveCard.SerializedCustomerCardType.DiscountBar));
            }
            _cashRegister.ProcessPayment(pmt, pmtContent, isBonusPmt, CurrentCustomer, MatchedAction == null ? Guid.Empty : MatchedAction.Id, pm =>
            {
                if (pm.Success)
                {
                    RefreshDataAsync();
                }

                else
                {
                    if (!String.IsNullOrEmpty(pm.Description))
                    {
                        ExtraWindow.Alert(new DialogParameters
                        {
                            Header = UIControls.Localization.Resources.UnableToProcess,
                            Content = pm.Description,
                            OkButtonContent = UIControls.Localization.Resources.Ok,
                            Owner = Application.Current.MainWindow
                        });
                    }
                }
                onFinish(pm);
            });
        }

        internal bool GetBonusVisibility()
        {
            if (GetAmountOfItemsInBasket() == 0) return false;
            return (DivisionGoods.Count(g => g.InBasket > 0 && g.BonusPrice.HasValue) == DivisionGoods.Count(g => g.InBasket > 0));
        }

        internal void AddToCustomersList(Customer customer)
        {
            if (!PresentCustomers.Any(c => c.Id == customer.Id))
            {
                PresentCustomers.Add(customer);
                PresentCustomersView.Refresh();
            }
            CurrentCustomer = PresentCustomers.FirstOrDefault(c => c.Id == customer.Id);
        }

        internal void UpdateActionText()
        {
            ActionText = String.Empty;
            MatchedAction = null;
            if (CurrentCustomer == null || CurrentCustomer.IsEmployee) return;
            if (GetAmountOfItemsInBasket() > 0)
            {
                foreach (var action in CurrentActions.Where(i => i.IsActive))
                {
                    if (!IsActionApplyable(action)) continue;
                    ActionText += String.Format("\n{3} \"{0}\" (-{2:p}): {1}", action.Name, action.GoodsList, action.Discount, UIControls.Localization.Resources.OfferAllowed);
                }
                if (ActionText.Length > 1)
                {
                    ActionText = ActionText.Substring(1);
                }
            }
            OnPropertyChanged("ActionText");
        }

        private bool IsActionApplyable(GoodAction action)
        {
            GoodAction preMatch = action;
            foreach (var i in action.SerializedGoodActions)
            {
                if (!DivisionGoods.Any(g => (g.Amount + g.InBasket) >= i.Value && g.GoodId == i.Key)) return false;
            }
            foreach (var good in DivisionGoods.Where(g => g.InBasket > 0))
            {
                if (!action.SerializedGoodActions.ContainsKey(good.GoodId)) return false;
                if (good.InBasket > action.SerializedGoodActions[good.GoodId]) return false;
                if (good.InBasket != action.SerializedGoodActions[good.GoodId])
                {
                    preMatch = null;
                }
            }
            if (GetAmountOfPositionsInBasket() != action.SerializedGoodActions.Count)
            {
                preMatch = null;
            }
            MatchedAction = preMatch;
            return true;
        }

        internal void UpdateStatus()
        {
            OnPropertyChanged("TotalPositions");
            OnPropertyChanged("TotalItems");
            OnPropertyChanged("TotalAmount");
            UpdateActionText();
        }

        TurnoverLargeViewModel _turnoverModel;
        public void RefreshDataAsync(TurnoverLargeViewModel turnoverModel)
        {
            _turnoverModel = turnoverModel;
            RefreshDataAsync();
        }
    }
}
