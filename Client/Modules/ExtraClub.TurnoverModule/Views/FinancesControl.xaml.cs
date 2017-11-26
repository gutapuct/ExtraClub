using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExtraClub.Infrastructure;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using ExtraClub.TurnoverModule.ViewModels;
using ExtraClub.UIControls;

namespace ExtraClub.TurnoverModule.Views
{
    public partial class FinancesControl
    {
        TurnoverLargeViewModel _model;
        ISettingsManager _setMan;


        public FinancesControl(TurnoverLargeViewModel model, ISettingsManager setMan)
        {

            InitializeComponent();
            _setMan = setMan;
            _setMan.RegisterGridView(this, ProviderPaymentsView);
            DataContext = _model = model;

            NavigationManager.SpendingRequest += new EventHandler<GuidEventArgs>(NavigationManager_SpendingRequest);
        }


        void NavigationManager_SpendingRequest(object sender, GuidEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(300);
                Dispatcher.Invoke(new Action(() =>
                {
                    FinansesTabs.SelectedIndex = 3;
                    _model.SpendingsStart = DateTime.Today.AddMonths(-4);
                    var sp = _model._spendings.FirstOrDefault(i => i.Id == e.Guid);
                    if (sp != null)
                    {
                        _model.SpendingsView.MoveCurrentTo(sp);
                    }
                }), new object[0]);
            });
        }

    }
}
