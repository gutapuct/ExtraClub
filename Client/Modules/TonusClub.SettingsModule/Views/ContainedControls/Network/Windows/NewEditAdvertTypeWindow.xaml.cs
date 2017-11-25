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
using TonusClub.ServiceModel;
using TonusClub.UIControls;
using TonusClub.UIControls.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls.Network.Windows
{
    public partial class NewEditAdvertTypeWindow
    {
        public AdvertType Type { get; set; }
        public List<AdvertGroup> Groups { get; set; }

        public NewEditAdvertTypeWindow(ClientContext context, AdvertType type, Guid? groupId = null)
            : base(context)
        {
            Groups = context.GetAdvertGroups();
            Type = type ?? new AdvertType { Id = Guid.NewGuid(), AdvertGroupId = groupId ?? Groups[0].Id };
            DataContext = this;
            InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Type.Name)) return;
            try
            {
                _context.PostAdvertType(Type.Id, Type.Name, Type.CommentNeeded, Type.AdvertGroupId);
            }
            catch(Exception ex)
            {
                TonusWindow.Alert("Ошибка", ex.Message);
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}
