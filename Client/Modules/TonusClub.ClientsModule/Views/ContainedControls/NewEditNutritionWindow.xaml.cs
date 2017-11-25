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
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;
using System.ComponentModel;

namespace TonusClub.Clients.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for NewEditNutritionWindow.xaml
    /// </summary>
    public partial class NewEditNutritionWindow
    {
        public Nutrition Nutrition { get; set; }

        public Customer Customer { get; set; }

        private List<string> dietTemplates = new List<string>();
        public ICollectionView DietTemplates { get; set; }
        private List<string> productTemplates = new List<string>();
        public ICollectionView ProductTemplates { get; set; }

        public NewEditNutritionWindow(Customer customer, Nutrition nutrition)
        {
            InitializeComponent();
            
            var obj = _context.GetNutritionTemplates();

            dietTemplates = obj[0];
            DietTemplates = CollectionViewSource.GetDefaultView(dietTemplates);
            productTemplates = obj[1];
            ProductTemplates = CollectionViewSource.GetDefaultView(productTemplates);

            Customer = customer;

            if (nutrition == null || nutrition.Id == Guid.Empty)
            {
                Nutrition = new Nutrition
                {
                    CreatedOn = DateTime.Today,
                    CustomerId = customer.Id,
                    Date = DateTime.Now,
                };
            }
            else
            {
                Nutrition = nutrition;
            }

            Customer = customer;
            this.DataContext = this;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            var res = _context.PostNutrition(Nutrition);

            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
