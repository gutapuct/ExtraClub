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
using TonusClub.ServiceModel.Reports;
using TonusClub.Infrastructure;
using TonusClub.ServiceModel;
using Microsoft.Practices.Unity;
using TonusClub.UIControls;

namespace TonusClub.Reports.Views.ContainedControls.ReportDesigner
{
    /// <summary>
    /// Interaction logic for ClauseControl.xaml
    /// </summary>
    public partial class ClauseControl : UserControl
    {
        public ClauseControl(Clause clause)
            : base()
        {
            DataContext = clause;
            InitializeComponent();
            InitPart(LeftPartControl, clause.LeftPart);
            InitPart(RightPartControl, clause.RightPart);
            
            clause.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(clause_PropertyChanged);
        }

        void clause_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var clause = sender as Clause;
            if (e.PropertyName == "LeftPart")
            {
                InitPart(LeftPartControl, clause.LeftPart);
            }
            else if (e.PropertyName == "RightPart")
            {
                InitPart(RightPartControl, clause.RightPart);
            }
        }

        public static void InitPart(ContentControl part, Clause clause)
        {
            if (!clause.IsFinite)
            {
                part.Content = new ClauseControl(clause);
            }
            else
            {
                part.Content = new FiniteClauseControl(clause, ApplicationDispatcher.UnityContainer.Resolve<ClientContext>());
            }
        }

        private void RemoveLeftButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as Clause).RemoveMe(true);
        }
        private void RemoveRightButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as Clause).RemoveMe(false);
        }
    }
}
