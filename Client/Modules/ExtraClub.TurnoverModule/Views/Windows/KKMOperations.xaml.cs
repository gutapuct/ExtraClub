using System.Windows;
using ExtraClub.CashRegisterModule;
using Microsoft.Practices.Unity;
using ExtraClub.UIControls;

namespace ExtraClub.TurnoverModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for KKMOperations.xaml
    /// </summary>
    public partial class KKMOperations
    {
        CashRegisterManager _cashMan;
        IUnityContainer _cont;
        public KKMOperations(CashRegisterManager cashMan, ClientContext context, IUnityContainer cont)
        {
            InitializeComponent();
            _cont = cont;
            _cashMan = cashMan;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            _cashMan.OpenShift();
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _cashMan.CloseShift();
            Close();
        }

        private void x1_Click(object sender, RoutedEventArgs e)
        {
            _cashMan.PrintReport(1);
            Close();
        }

        private void x2_Click(object sender, RoutedEventArgs e)
        {
            _cashMan.PrintReport(2);
            Close();
        }

        private void z1_Click(object sender, RoutedEventArgs e)
        {
            _cashMan.PrintReport(3);
            Close();
        }

        private void returnClick(object sender, RoutedEventArgs e)
        {
            _cont.Resolve<FRReturnWindow>().ShowDialog();
        }

        private void closeClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
