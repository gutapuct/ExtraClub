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
using ExtraClub.UIControls.Windows;

namespace ExtraClub.Clients.Views.Windows
{
    public partial class CompensateWindow
    {
        private bool _pos1 = true, _pos2, _pos3, _pos1a, _pos1b, _pos2a, _pos2b;
        public Boolean Pos1
        {
            get
            {
                return _pos1;
            }
            set
            {
                _pos1 = value;
                OnPropertyChanged("Pos1");
            }
        }
        public Boolean Pos2
        {
            get
            {
                return _pos2;
            }
            set
            {
                _pos2 = value;
                OnPropertyChanged("Pos2");
            }
        }
        public Boolean Pos3
        {
            get
            {
                return _pos3;
            }
            set
            {
                _pos3 = value;
                OnPropertyChanged("Pos3");
            }
        }

        public Boolean Pos1a
        {
            get
            {
                return _pos1a;
            }
            set
            {
                _pos1a = value;
                OnPropertyChanged("Pos1a");
            }
        }
        public Boolean Pos1b
        {
            get
            {
                return _pos1b;
            }
            set
            {
                _pos1b = value;
                OnPropertyChanged("Pos1b");
            }
        }
        public Boolean Pos2a
        {
            get
            {
                return _pos2a;
            }
            set
            {
                _pos2a = value;
                OnPropertyChanged("Pos2a");
            }
        }
        public Boolean Pos2b
        {
            get
            {
                return _pos2b;
            }
            set
            {
                _pos2b = value;
                OnPropertyChanged("Pos2b");
            }
        }

        public Customer Customer { get; set; }

        public CompensateWindow(ClientContext context, Customer customer)
            : base(context)
        {
            Customer = customer;
            this.DataContext = this;
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            if(!Pos1 && !Pos2 && !Pos3)
            {
                return;
            }
            if(Pos1 && !Pos1a && !Pos1b)
            {
                return;
            }
            if(Pos2 && !Pos2a && !Pos2b)
            {
                return;
            }
            if(Pos1 && Pos1a)
            {
                _context.PostBonusCorrection(Customer.Id, 120, "Форс-мажор");
            }
            if(Pos1 && Pos1b)
            {
                if (!_context.PostExtraSmart(Customer.Id, "Форс-мажор"))
                {
                    ExtraWindow.Alert("Ошибка", "Не найдено ни одного активного смарт-абонемента. Компенсация смарт-тренировкой невозможна.");
                    return;
                }
            }
            if(Pos2 && Pos2a)
            {
                _context.PostBonusCorrection(Customer.Id, 200, "Качество сервиса");
            }
            if(Pos2 && Pos2b)
            {
                if (!_context.PostExtraSmart(Customer.Id, "Качество сервиса"))
                {
                    ExtraWindow.Alert("Ошибка", "Не найдено ни одного активного смарт-абонемента. Компенсация смарт-тренировкой невозможна.");
                    return;
                }

            }
            if(Pos3)
            {
                _context.PostBonusCorrection(Customer.Id, 50, "Работа клуба");
            }
            DialogResult = true;
            Close();
        }
    }
}
