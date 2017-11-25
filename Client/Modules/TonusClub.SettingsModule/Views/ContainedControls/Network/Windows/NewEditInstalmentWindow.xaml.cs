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
using TonusClub.Infrastructure.Interfaces;
using TonusClub.UIControls;
using TonusClub.UIControls.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls.Network.Windows
{
    public partial class NewEditInstalmentWindow
    {
        public Instalment Instalment { get; set; }
        public object SettingsFolders { get; set; }

        private bool isTwo = true;

        public Visibility SecVis
        {
            get
            {
                return isTwo ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public bool TwoPayments
        {
            get
            {
                return isTwo;
            }
            set
            {
                isTwo = value;
                firstLength.Text = "Длительность (дней)";
                OnPropertyChanged("TwoPayments");
                OnPropertyChanged("ThreePayments");
                OnPropertyChanged("SecVis");
            }
        }

        public bool ThreePayments
        {
            get
            {
                return !isTwo;
            }
            set
            {
                isTwo = !value;
                firstLength.Text = "Длительность первого периода (дней)";
                OnPropertyChanged("TwoPayments");
                OnPropertyChanged("ThreePayments");
                OnPropertyChanged("SecVis");
            }
        }

        public NewEditInstalmentWindow(ClientContext context, Instalment instalment)
            : base(context)
        {
            SettingsFolders = context.GetSettingsFolders(2, false);
            Instalment = instalment;
            InitializeComponent();
            if (instalment.ContribPercent.HasValue) instalment.ContribPercent *= 100;
            instalment.AvailableUnitsPercent *= 100;
            instalment.SecondPercent *= 100;
            isTwo = !instalment.SecondPercent.HasValue;
            DataContext = this;
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Instalment.Name))
            {
                TonusWindow.Alert("Ошибка", "Укажите название рассрочки!");
                return;
            }
            if (Instalment.ContribAmount.HasValue && Instalment.ContribPercent.HasValue)
            {
                TonusWindow.Alert("Ошибка", "Можно указать только один тип первого взноса!");
                return;
            }
            if (!Instalment.ContribAmount.HasValue && !Instalment.ContribPercent.HasValue)
            {
                TonusWindow.Alert("Ошибка", "Необходимо указать размер первого взноса!");
                return;
            }

            if (isTwo)
            {
                Instalment.SecondLength = null;
                Instalment.SecondPercent = null;
            }
            else
            {
                if (Instalment.SecondLength == null)
                {
                    TonusWindow.Alert("Ошибка", "Необходимо указать вторую длительность!");
                    return;
                }
                if (Instalment.SecondPercent == null)
                {
                    TonusWindow.Alert("Ошибка", "Необходимо указать размер второго взноса!");
                    return;
                }
            }

            if (Instalment.ContribPercent.HasValue) Instalment.ContribPercent /= 100;
            Instalment.AvailableUnitsPercent /= 100;
            Instalment.SecondPercent /= 100;

            _context.PostInstalment(Instalment);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
