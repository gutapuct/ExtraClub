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
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.Clients.Views.Windows.CustomerAndCards
{
    public partial class AddBonusWindow
    {
        public Customer Customer { get; set; }

        private int _Amount;
        public int Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                _Amount = value;
                OnPropertyChanged("Amount");
                OnPropertyChanged("IsPostEnabled");
            }
        }

        private string _Description;
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
                OnPropertyChanged("Description");
            }
        }

        public bool IsPostEnabled
        {
            get
            {
                return Amount > 0;
            }
        }

        private bool AddBonus { get; set; }

        public AddBonusWindow(ClientContext context, Customer customer, bool addBonus)
            : base(context)
        {
            InitializeComponent();

            Customer = customer;
            AddBonus = addBonus;
            if(addBonus)
            {
                Title = "Начисление бонусов";
            }
            else
            {
                Title = "Списание бонусов";
            }
            DataContext = this;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostBonusCorrection(Customer.Id, Amount * (AddBonus ? 1 : -1), Description);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
