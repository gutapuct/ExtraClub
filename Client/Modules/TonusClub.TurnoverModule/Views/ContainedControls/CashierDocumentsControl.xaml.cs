using Microsoft.Practices.Unity;
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
using TonusClub.TurnoverModule.ViewModels;

namespace TonusClub.TurnoverModule.Views.ContainedControls
{
    public partial class CashierDocumentsControl
    {
        private CashierDocumentsViewModel Model
        {
            get
            {
                return (CashierDocumentsViewModel)DataContext;
            }
        }

        public CashierDocumentsControl(object model)
        {
            DataContext = model;
            InitializeComponent();
        }
    }
}
