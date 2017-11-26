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
using Telerik.Windows.Controls;
using ExtraClub.Infrastructure.Interfaces;
using System.ComponentModel;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;
using ViewModelBase = ExtraClub.UIControls.BaseClasses.ViewModelBase;

namespace ExtraClub.SettingsModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for NewEditTreatmentWindow.xaml
    /// </summary>
    public partial class NewEditCardTypeWindow
    {
        private IDictionaryManager _dictMan;

        public CustomerCardType Card { get; set; }

        public List<SettingsFolder> SettingsFolders { get; set; }

        public NewEditCardTypeWindow(IDictionaryManager dictMan, ClientContext context, CustomerCardType cardType, bool readOnly)
        {
            SettingsFolders = context.GetSettingsFolders(0, false);

            Owner = Application.Current.MainWindow;
            _dictMan = dictMan;

            if (cardType == null || cardType.Id == Guid.Empty)
            {
                Card = new CustomerCardType
                {
                    IsActive = true,
                    GiveBonusForCards = true
                };
            }
            else
            {
                Card = cardType;

            }

            InitializeComponent();

            if (!Card.IsActive)
            {
                RestoreButton.Visibility = System.Windows.Visibility.Visible;
                CommitButton.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (readOnly)
            {
                Card = ViewModelBase.Clone<CustomerCardType>(Card);
                CommitButton.IsEnabled = false;
                RestoreButton.IsEnabled = false;
            }

            DataContext = this;

        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Card.Error)) return;
            _context.PostCustomerCardType(Card);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            _context.SetObjectActive("CustomerCardTypes", Card.Id, true);
            DialogResult = true;
            Close();
        }
    }
}
