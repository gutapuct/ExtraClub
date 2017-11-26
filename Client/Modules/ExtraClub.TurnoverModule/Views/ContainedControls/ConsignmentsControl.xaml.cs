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
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.TurnoverModule.Views.ContainedControls
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
