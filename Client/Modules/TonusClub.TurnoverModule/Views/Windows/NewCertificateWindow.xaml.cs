using System;
using System.Windows;
using TonusClub.ServiceModel;
using TonusClub.Infrastructure.Interfaces;
using System.ComponentModel;
using TonusClub.UIControls;

namespace TonusClub.TurnoverModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for NewCertificateWindow.xaml
    /// </summary>
    public partial class NewCertificateWindow
    {
        public Certificate Certificate { get; set; }
        public ICollectionView GoodCategories { get; private set; }

        public NewCertificateWindow(IDictionaryManager dictManager, ClientContext context)
        {
            GoodCategories = dictManager.GetViewSource("GoodsCategories");
            Certificate = new Certificate
            {
                AuthorId = context.CurrentUser.UserId,
                CompanyId = context.CurrentCompany.CompanyId,
                CreatedOn = DateTime.Now,
                DivisionId = context.CurrentDivision.Id,
                Id = Guid.NewGuid()
            };
            DataContext = this;
            InitializeComponent();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Certificate.Error)) return;
            _context.PostCertificate(Certificate);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
