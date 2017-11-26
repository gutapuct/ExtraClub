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
using ExtraClub.ServiceModel.Reports;
using ExtraClub.Infrastructure;
using ExtraClub.ServiceModel;
using Microsoft.Practices.Unity;
using ExtraClub.UIControls;

namespace ExtraClub.Reports.Views.ContainedControls.ReportDesigner
{
    /// <summary>
    /// Interaction logic for ClauseContainer.xaml
    /// </summary>
    public partial class ClauseContainer : UserControl
    {
        public Clause CurrentChain;

        public Type BaseType { get; set; }

        public ClauseContainer(Type baseType, Clause baseClause = null)
        {
            if (baseClause == null) baseClause = new FiniteClause(baseType);
            //if (baseClause is FiniteClause && (baseClause as FiniteClause).BaseType == null)
            //{
            //    (baseClause as FiniteClause).BaseType = baseType;
            //    (baseClause as FiniteClause).Parameter = (baseClause as FiniteClause).Parameter;
            //}
            CurrentChain = baseClause;
            CurrentChain.Replace += _base_Replace;
            CurrentChain.PropertyChanged += baseClause_PropertyChanged;
            BaseType = baseType;
            InitializeComponent();
            if (baseClause is FiniteClause)
            {
                ClauseContent.Content = new FiniteClauseControl(baseClause, ApplicationDispatcher.UnityContainer.Resolve<ClientContext>());
            }
            else
            {
                ClauseContent.Content = new ClauseControl(baseClause);

            }
        }

        void _base_Replace(object sender, ClauseArgs e)
        {
            CurrentChain.Replace -= _base_Replace;
            CurrentChain.PropertyChanged -= baseClause_PropertyChanged;
            ClauseControl.InitPart(ClauseContent, e.NewClause);
            CurrentChain = e.NewClause;
            CurrentChain.Replace += _base_Replace;
            CurrentChain.PropertyChanged += baseClause_PropertyChanged;
        }

        void baseClause_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LeftPart" || e.PropertyName == "RightPart")
            {
                ClauseControl.InitPart(ClauseContent, CurrentChain);
            }
            if (ClauseChanged != null)
            {
                ClauseChanged(this, null);
            }
        }

        public event EventHandler ClauseChanged;

        private void OuterReplace(Clause outer)
        {
            CurrentChain.Replace -= _base_Replace;
            CurrentChain.PropertyChanged -= baseClause_PropertyChanged;
            outer.AttachLeft(CurrentChain);
            outer.AttachRight(new FiniteClause(BaseType));
            CurrentChain = outer;
            CurrentChain.Replace += _base_Replace;
            CurrentChain.PropertyChanged += baseClause_PropertyChanged;
            ClauseControl.InitPart(ClauseContent, outer);
            if (ClauseChanged != null)
            {
                ClauseChanged(this, null);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OuterReplace(new AndClause());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OuterReplace(new OrClause());
        }
    }
}
