using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TonusClub.UIControls;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;

namespace TonusClub.TurnoverModule.Views
{
    /// <summary>
    /// Interaction logic for NewGoodActionWindow.xaml
    /// </summary>
    public partial class NewGoodActionWindow : WindowBase
    {
        #region DataContext

        private string _actionName;
        public string ActionName { get { return _actionName; } set { _actionName = value; OnPropertyChanged("ActionName"); } }

        private double _discount;
        public double Discount { get { return _discount; } set { _discount = value; OnPropertyChanged("Discount"); } }

        public List<Good> GoodsList { get; private set; }

        private bool _isActive = false;
        public bool IsActionActive { get { return _isActive; } set { _isActive = value; OnPropertyChanged("IsActionActive"); } }

        private Guid _actionId;

        #endregion

        public NewGoodActionWindow(ClientContext context, IDictionaryManager dictMan) : base(context)
        {
            DataContext = this;

            GoodsList = _context.GetAllGoods();

            InitializeComponent();
        }

        public void ApplyEdit(GoodAction action)
        {
            ActionName = action.Name;
            Discount = (double)action.Discount * 100;
            IsActionActive = action.IsActive;
            foreach (var good in GoodsList)
            {
                if (action.SerializedGoodActions.ContainsKey(good.Id))
                {
                    good.Amount = action.SerializedGoodActions[good.Id];
                }
            }
            _actionId = action.Id;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(ActionName)) return;

            var sel = new List<KeyValuePair<Guid, int>>();
            foreach (var good in GoodsList.Where(g => g.Amount > 0))
            {
                sel.Add(new KeyValuePair<Guid, int>(good.Id, good.Amount));
            }

            if (sel.Count() == 0) return;

            _context.PostGoodAction(_actionId, ActionName, Discount/100, sel, IsActionActive);

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
