using System;
using System.Windows;
using ExtraClub.ServiceModel;
using ExtraClub.Infrastructure.Interfaces;
using System.ComponentModel;
using ExtraClub.UIControls;

namespace ExtraClub.TurnoverModule.Views.Windows
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
