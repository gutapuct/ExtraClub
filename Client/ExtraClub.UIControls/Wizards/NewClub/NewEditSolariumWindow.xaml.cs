using System;
using System.Windows;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.ClientDal.Wizards.NewClub
{
    /// <summary>
    /// Interaction logic for NewEditSolariumWindow.xaml
    /// </summary>
    public partial class NewEditSolariumWindow
    {
        public Solarium Solarium{ get; set; }


        public NewEditSolariumWindow(ClientContext context, Solarium solarium, Guid divId)
        {
            DataContext = this;
            //Owner = Application.Current.MainWindow;

            if (solarium == null || solarium.Id == Guid.Empty)
            {
                Solarium = new Solarium
                {
                    AuthorId = context.CurrentUser.UserId,
                    CreatedOn = DateTime.Now,
                    DivisionId = divId,
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
