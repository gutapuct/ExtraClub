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
using System.ComponentModel;

namespace TonusClub.OrganizerModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for CustomerTargetWindow.xaml
    /// </summary>
    public partial class CustomerTargetWindow : INotifyPropertyChanged
    {
        public CustomerTarget Target { get; set; }
        public Customer Customer { get; set; }

        public CustomerTargetWindow(CustomerTarget item)
        {
            InitializeComponent();

            Target = item;
            Customer = _context.GetCustomer(Target.CustomerId);

            Target.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Target_PropertyChanged);

            DataContext = this;
        }

        void Target_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged("IsUnmatchEnabled");
            OnPropertyChanged("IsMatchEnabled");
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Target.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Target_PropertyChanged);
        }

        public bool IsUnmatchEnabled
        {
            get
            {
                return Target.RecomendationsFollowed.HasValue && !String.IsNullOrWhiteSpace(Target.Comment);
            }
        }

        public bool IsMatchEnabled
        {
            get
            {
                return Target.RecomendationsFollowed.HasValue;
            }
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UnmatchButton_Click(object sender, RoutedEventArgs e)
        {
            Target.TargetComplete = false;
            _context.PostCustomerTarget(Target);
            DialogResult = true;
            Close();
        }

        private void MatchButton_Click(object sender, RoutedEventArgs e)
        {
            Target.TargetComplete = true;
            _context.PostCustomerTarget(Target);
            DialogResult = true;
            Close();
        }
    }
}
