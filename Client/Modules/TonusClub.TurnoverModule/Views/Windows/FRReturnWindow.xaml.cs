using System;
using System.Collections.Generic;
using System.Windows;
using TonusClub.CashRegisterModule;
using TonusClub.UIControls.Windows;
using TonusClub.Infrastructure;
using TonusClub.UIControls;

namespace TonusClub.TurnoverModule.Views.Windows
{
    public partial class FRReturnWindow
    {
        public decimal RefundAmount { get; set; }
        public int Section { get; set; }

        public Dictionary<int, string> Sections { get; set; }


        public Visibility SectVis { get; set; }

        CashRegisterManager _cashMan;

        public FRReturnWindow(CashRegisterManager cashMan, ClientContext context)
            : base(context)
        {
            _cashMan = cashMan;
            Section = 1;
            InitializeComponent();

            SectVis = AppSettingsManager.GetSetting("SectionsNumber") == "1" ? Visibility.Collapsed : Visibility.Visible;

            var sects = Int32.Parse(AppSettingsManager.GetSetting("SectionsNumber"));
            if (sects > 4 || sects < 2)
            {
                Sections = new Dictionary<int, string> { { 1, "1" } };
            }
            else
            {
                Sections = new Dictionary<int, string>();
                for (int i = 1; i <= sects; i++)
                {
                    Sections.Add(i, i.ToString());
                }
            }

            DataContext = this;

        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (RefundAmount > 0 && Section > 0)
            {
                _cashMan.PrintReturn(RefundAmount, _context.CurrentUser.FullName, Section);
            }
            else
            {
                TonusWindow.Alert(UIControls.Localization.Resources.Error, UIControls.Localization.Resources.IncorrectAmountMessage);
            }

            Close();
        }
    }
}