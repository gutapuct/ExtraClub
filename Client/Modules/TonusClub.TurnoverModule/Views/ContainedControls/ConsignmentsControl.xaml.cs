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
using TonusClub.TurnoverModule.ViewModels;
using TonusClub.TurnoverModule.Views.Windows;
using TonusClub.ServiceModel;
using TonusClub.Infrastructure.BaseClasses;
using TonusClub.UIControls.BaseClasses;

namespace TonusClub.TurnoverModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for ConsignmentsControl.xaml
    /// </summary>
    public partial class ConsignmentsControl
    {
        private TurnoverLargeViewModel Model
        {
            get
            {
                return (TurnoverLargeViewModel)DataContext;
            }
        }

        public ConsignmentsControl()
        {
            InitializeComponent();
        }

        private void NewConsignment_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewConsignment>(() => Model.RefreshConsignments());
        }

        private void NewMoveConsignment_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditMoveConsignment>(() => Model.RefreshConsignments());
        }

        private void NewOutConsignment_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditDeductionConsignment>(() => Model.RefreshConsignments(), new ParameterOverride("rashod", false));
        }

        private void ConsignmentsView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                if (Model.ConsignmentsView.CurrentItem != null)
                {
                    var doc = Model.ConsignmentsView.CurrentItem as Consignment;
                    if (doc.DocType == 0)
                    {
                        ProcessUserDialog<NewConsignment>(()=>Model.RefreshConsignments(),new ResolverOverride[] { new ParameterOverride("consignment", ViewModelBase.Clone<Consignment>(doc)) });
                    }
                    if (doc.DocType == 1)
                    {
                        ProcessUserDialog<NewEditMoveConsignment>(()=>Model.RefreshConsignments(), new ResolverOverride[] { new ParameterOverride("consignment", ViewModelBase.Clone<Consignment>(doc)) });
                    }
                    if (doc.DocType == 2)
                    {
                        var iswriteoff = false;
#if BEAUTINIKA
                        iswriteoff=true;
#endif
                        ProcessUserDialog<NewEditDeductionConsignment>(() => Model.RefreshConsignments(), new ResolverOverride[] { new ParameterOverride("consignment", ViewModelBase.Clone<Consignment>(doc)), new ParameterOverride("rashod", iswriteoff) });
                    }          
                }
            }
        }

        private void NewRasConsignment_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditDeductionConsignment>(() => Model.RefreshConsignments(), new ParameterOverride("rashod", true));
        }
    }
}
