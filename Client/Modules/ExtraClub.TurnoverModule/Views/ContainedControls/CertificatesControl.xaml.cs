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
using Microsoft.Practices.Unity;
using ExtraClub.TurnoverModule.ViewModels;
using ExtraClub.TurnoverModule.Views.Windows;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.TurnoverModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for CertificatesControl.xaml
    /// </summary>
    public partial class CertificatesControl
    {
        private TurnoverLargeViewModel Model
        {
            get
            {
                return (TurnoverLargeViewModel)DataContext;
            }
        }

        public CertificatesControl()
        {
            InitializeComponent();
        }

        private void EmitCertificateClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewCertificateWindow>(()=>Model.RefreshCertificates());
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CertificatesView.CurrentItem != null)
            {
                ExtraWindow.Confirm(UIControls.Localization.Resources.DeleteSelectedM,
                    UIControls.Localization.Resources.DeleteCertMessage, e1 =>
                {
                    if (e1.DialogResult ?? false)
                    {
                        if (ClientContext.CancelCertificate(((Certificate)Model.CertificatesView.CurrentItem).Id))
                        {
                            Model.RefreshCertificates();
                        }
                    }
                });
            }
        }
    }
}
