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
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows
{
    public partial class NewEditCumulativeWindow
    {
        public CumulativeDiscount Cumulative { get; set; }

        public NewEditCumulativeWindow(ClientContext context, CumulativeDiscount cumulative)
            : base(context)
        {
            InitializeComponent();

            if (cumulative != null && cumulative.Id != Guid.Empty)
            {
                Cumulative = cumulative;
            }
            else
            {
                Cumulative = new CumulativeDiscount
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
            if (Cumulative.ValueFrom > Cumulative.ValueTo) return;
            _context.PostCumulative(Cumulative);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
