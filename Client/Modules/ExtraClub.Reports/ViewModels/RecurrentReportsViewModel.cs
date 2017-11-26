using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.Reports.ViewModels
{
    public class RecurrentReportsViewModel : ViewModelBase
    {
        private List<ReportRecurrency> _RecurrentReports = new List<ReportRecurrency>();
        public ICollectionView RecurrentReportsView { get; set; }
        

        public RecurrentReportsViewModel(IUnityContainer container)
            : base()
        {
            RecurrentReportsView = CollectionViewSource.GetDefaultView(_RecurrentReports);
        }

        protected override void RefreshDataInternal()
        {
            _RecurrentReports.Clear();
            _RecurrentReports.AddRange(ClientContext.GetRecurrentReports());
        }

        protected override void RefreshFinished()
        {
            base.RefreshFinished();
            RecurrentReportsView.Refresh();
        }
    }
}
