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
using TonusClub.UIControls;

namespace TonusClub.SettingsModule.Views.ContainedControls.Club
{
    /// <summary>
    /// Interaction logic for NewEditSolariumWindow.xaml
    /// </summary>
    public partial class NewEditSolariumWindow
    {
        public Solarium Solarium{ get; set; }
        public List<CompanySettingsFolder> SettingsFolders { get; set; }


        public NewEditSolariumWindow(ClientContext context, Solarium solarium):base(context)
        {
            SettingsFolders = context.GetCompanySettingsFolders(4);

            this.DataContext = this;
            Owner = Application.Current.MainWindow;

            if (solarium == null || solarium.Id == Guid.Empty)
            {
                Solarium = new Solarium
                {
                    AuthorId = context.CurrentUser.UserId,
                    CreatedOn = DateTime.Now,
                    DivisionId = context.CurrentDivision.Id,
                    IsActive = true
                };
            }
            else
            {
                Solarium = solarium;
            }
            InitializeComponent();
        }


        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostSolarium(Solarium);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
