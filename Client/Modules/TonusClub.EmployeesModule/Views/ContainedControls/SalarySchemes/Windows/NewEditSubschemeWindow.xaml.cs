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
using TonusClub.ServiceModel;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel.Employees;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using TonusClub.UIControls;

namespace TonusClub.EmployeesModule.Views.ContainedControls.SalarySchemes.Windows
{
    /// <summary>
    /// Interaction logic for NewEditSubschemeWindow.xaml
    /// </summary>
    public partial class NewEditSubschemeWindow
    {
        public SalarySchemeCoefficient CoeffResult { get; set; }

        public Dictionary<int, string> SubTypes { get; set; }

        public IEnumerable<TicketType> TicketTypes { get; set; }
        public IEnumerable<CustomerCardType> CardTypes { get; set; }

        public List<AdvertType> AdvertTypes { get; set; }
        public object CustStatuses { get; set; }

        public string PremUnit { get; set; }

        #region shit

        public bool d1
        {
            get { return GetBit(2); }
            set { SetBit(2, value); }
        }
        public bool d2
        {
            get { return GetBit(3); }
            set { SetBit(3, value); }
        }
        public bool d3
        {
            get { return GetBit(4); }
            set { SetBit(4, value); }
        }
        public bool d4
        {
            get { return GetBit(5); }
            set { SetBit(5, value); }
        }
        public bool d5
        {
            get { return GetBit(6); }
            set { SetBit(6, value); }
        }
        public bool d6
        {
            get { return GetBit(7); }
            set { SetBit(7, value); }
        }
        public bool d7
        {
            get { return GetBit(8); }
            set { SetBit(8, value); }
        }

        #endregion

        public ObservableCollection<SalaryRateTable> RateTable { get; set; }

        private bool GetBit(int index)
        {
            CoeffResult.Int1 = CoeffResult.Int1 ?? 0;
            return (CoeffResult.Int1 & (1 << index)) != 0;
        }


        private void SetBit(int index, bool value)
        {
            if (value)
                CoeffResult.Int1 |= (1 << index);
            else
                CoeffResult.Int1 &= ~(1 << index);
        }


        public object Categories { get; set; }
        public object Goods { get; set; }
        public object Actions { get; set; }
        public object TreatmentTypes { get; set; }
        public object Treatments { get; set; }
        public object Corporates { get; set; }

        public NewEditSubschemeWindow(ClientContext context, SalarySchemeCoefficient coeff, IDictionaryManager dictMan)
            : base(context)
        {
            SubTypes = CoefficientTypes.TypeNames;
            TicketTypes = context.GetTicketTypes(true);
            CardTypes = context.GetCustomerCardTypes(true);
            Categories = dictMan.GetViewSource("GoodsCategories");
            Goods = context.GetAllGoods();
            Actions = context.GetGoodActions(true);
            TreatmentTypes = context.GetAllTreatmentTypes().Where(i => i.IsActive).ToList();
            Treatments = context.GetAllTreatments().Where(o => o.IsActive);
            AdvertTypes = context.GetAdvertTypes().OrderBy(t => t.Name).ToList();
            RateTable = new ObservableCollection<SalaryRateTable>();
            RateTable.CollectionChanged += RateTable_CollectionChanged;
            CustStatuses = dictMan.GetViewSource("CustomerStatuses");
            Corporates = context.GetCorporates();

            PremUnit = "руб.";

            if (coeff == null || coeff.Id == Guid.Empty)
            {
                CoeffResult = new SalarySchemeCoefficient
                {
                    CompanyId = context.CurrentCompany.CompanyId,
                    Id = Guid.NewGuid(),
                    SerializedRateTable = new List<SalaryRateTable>()
                };
                InitRateTable();
            }
            else
            {
                CoeffResult = coeff;
                foreach (var t in coeff.SerializedRateTable.OrderBy(i => i.FromValue))
                {
                    RateTable.Add(t);
                }
                if (RateTable.Count > 0)
                {
                    RateTable[0].IsFirst = true;
                }
                else
                {
                    InitRateTable();
                }

                if (coeff.CoeffTypeId == 26)
                {
                    PremUnit = "% продаж";
                }
            }
            InitializeComponent();
#if BEAUTINIKA
            g17.Header = "Посещаемость студии по дням недели/часам";
#endif
            DataContext = this;

            CoeffResult.PropertyChanged += CoeffResult_PropertyChanged;
            CoeffResult_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("CoeffTypeId"));
        }

        void RateTable_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                ((SalaryRateTable)e.NewItems[0]).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(NewEditSubschemeWindow_PropertyChanged);
            }
        }

        void NewEditSubschemeWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var item = sender as SalaryRateTable;
            if (item == null) return;
            var pos = RateTable.ToList().IndexOf(item);
            if (item.IsFirst && e.PropertyName == "ToValue" && RateTable.Count == 1)
            {
                RateTable.Add(new SalaryRateTable { FromValue = item.ToValue });
                return;
            }
            if (RateTable.Count == pos + 1 && e.PropertyName == "ToValue")
            {
                RateTable.Add(new SalaryRateTable { FromValue = item.ToValue });
                return;
            }
            if (e.PropertyName == "FromValue" && pos > 0 && item.FromValue.HasValue)
            {
                RateTable[pos-1].ToValue = item.FromValue;
                return;
            }
            if (e.PropertyName == "ToValue" && pos < (RateTable.Count - 1) && item.ToValue.HasValue)
            {
                RateTable[pos + 1].FromValue = item.ToValue;
                return;
            }
        }

        private void InitRateTable()
        {
            RateTable.Add(new SalaryRateTable { IsFirst = true, FromValue = 0 });
        }

        void CoeffResult_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CoeffTypeId")
            {
                g2.Visibility = CoeffResult.CoeffTypeId == 2 ? Visibility.Visible : Visibility.Collapsed;
                g4.Visibility = CoeffResult.CoeffTypeId == 4 ? Visibility.Visible : Visibility.Collapsed;
                g6.Visibility = CoeffResult.CoeffTypeId == 6 ? Visibility.Visible : Visibility.Collapsed;
                g7.Visibility = CoeffResult.CoeffTypeId == 7 ? Visibility.Visible : Visibility.Collapsed;
                g8.Visibility = CoeffResult.CoeffTypeId == 8 ? Visibility.Visible : Visibility.Collapsed;
                g9.Visibility = CoeffResult.CoeffTypeId == 9 ? Visibility.Visible : Visibility.Collapsed;
                g10.Visibility = CoeffResult.CoeffTypeId == 10 ? Visibility.Visible : Visibility.Collapsed;
                g15.Visibility = CoeffResult.CoeffTypeId == 15 ? Visibility.Visible : Visibility.Collapsed;
                g16.Visibility = CoeffResult.CoeffTypeId == 16 ? Visibility.Visible : Visibility.Collapsed;
                g17.Visibility = CoeffResult.CoeffTypeId == 17 ? Visibility.Visible : Visibility.Collapsed;
                g18.Visibility = CoeffResult.CoeffTypeId == 18 ? Visibility.Visible : Visibility.Collapsed;
                g2021.Visibility = (CoeffResult.CoeffTypeId == 20 || CoeffResult.CoeffTypeId == 21) ? Visibility.Visible : Visibility.Collapsed;
                g22p.Visibility = (CoeffResult.CoeffTypeId >= 22 && CoeffResult.CoeffTypeId < 25) ? Visibility.Visible : Visibility.Collapsed;
            }
            if (CoeffResult.CoeffTypeId == 26)
            {
                PremUnit = "% продаж";
            }
            else
            {
                PremUnit = "руб.";
            }
            OnPropertyChanged("PremUnit");
        }

        protected override void OnClosed(EventArgs e)
        {
            CoeffResult.PropertyChanged -= CoeffResult_PropertyChanged;
            base.OnClosed(e);
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var row in RateTable)
            {
                if (row.Error != null) return;
            }
            CoeffResult.SerializedRateTable.Clear();
            foreach (var t in RateTable)
            {
                CoeffResult.SerializedRateTable.Add(t);
            }
            DialogResult = true;
            Close();
        }

        private void RemoveRateClick(object sender, RoutedEventArgs e)
        {
            var item = ((sender) as RadButton).DataContext as SalaryRateTable;
            var pos = RateTable.ToList().IndexOf(item);
            if (pos == RateTable.Count - 1)
            {
                RateTable.Remove(item);
                RateTable[pos - 1].ToValue = null;
                return;
            }
            RateTable.Remove(item);
            RateTable[pos - 1].ToValue = RateTable[pos].FromValue;
        }
    }
}
