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

namespace ExtraClub.SettingsModule.Views.ContainedControls.Network.Windows
{
    public partial class NewEditNewsWindow
    {
        public News News { get; set; }

        public NewEditNewsWindow(ClientContext context, News news)
            : base(context)
        {
            if (news.Id == Guid.Empty)
            {
                News = new News
                {
                    CreatedBy = context.CurrentUser.UserId,
                    CreatedOn = DateTime.Today,
                    Id = Guid.NewGuid(),
                    Message = "",
                    Subject = ""
                };
            } else {
                News = news;
            }
            DataContext = this;
            InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostNews(News);
            DialogResult = true;
            Close();
        }
    }
}
