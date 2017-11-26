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
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Franch.Windows
{
    public partial class NewEditBarDiscountWindow
    {
        public BarDiscount BarDiscount { get; set; }

        public NewEditBarDiscountWindow(ClientContext context, BarDiscount discount)
            : base(context)
        {
            InitializeComponent();

            if (discount != null && discount.Id != Guid.Empty)
            {
                BarDiscount = discount;
            }
            else
            {
                BarDiscount = new BarDiscount
                {
                    CreatedOn = DateTime.Now,
                    CreatedById = _context.CurrentUser.UserId,
                    Id = Guid.NewGuid(),
                    CompanyId = _context.CurrentCompany.CompanyId
                };
            }


            DataContext = this;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (BarDiscount.ValueFrom > BarDiscount.ValueTo) return;
            _context.PostBarDiscount(BarDiscount);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
