using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtraClub.Infrastructure.BaseClasses;
using Microsoft.Practices.Unity;
using System.ComponentModel;
using ExtraClub.ServiceModel.Reports;
using System.Windows.Data;
using System.Windows;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.Reports.ViewModels
{
    public class ReportLargeViewModel : ViewModelBase
    {
        public ICollectionView ReportsView { get; set; }

        public List<ReportInfoInt> _reports = new List<ReportInfoInt>();

        private FontWeight _b1w = FontWeights.DemiBold;
        private FontWeight _b2w = FontWeights.Normal;
        private FontWeight _b3w = FontWeights.Normal;
        private FontWeight _b4w = FontWeights.Normal;

        public FontWeight Button1Weight
        {
            get
            {
                return _b1w;
            }
            set
            {
                _b1w = value;
                if (_b1w == FontWeights.DemiBold)
                {
                    Button2Weight = FontWeights.Normal;
                    Button3Weight = FontWeights.Normal;
                    Button4Weight = FontWeights.Normal;

                    ReportsView.Filter = delegate(object item)
                        {
                            return (item as ReportInfoInt).Type == ReportType.Stored || (item as ReportInfoInt).Type == ReportType.Code;
                        };
                    ReportsView.Refresh();
                    ReportsView.MoveCurrentToFirst();
                }
                OnPropertyChanged("Button1Weight");
            }
        }
        public FontWeight Button2Weight
        {
            get
            {
                return _b2w;
            }
            set
            {
                _b2w = value;
                if (_b2w == FontWeights.DemiBold)
                {
                    Button1Weight = FontWeights.Normal;
                    Button3Weight = FontWeights.Normal;
                    Button4Weight = FontWeights.Normal;

                    ReportsView.Filter = delegate(object item)
                    {
                        return (item as ReportInfoInt).Type == ReportType.StoredParams || (item as ReportInfoInt).Type == ReportType.CodeParams;
                    };
                    ReportsView.Refresh();
                    ReportsView.MoveCurrentToFirst();
                }
                OnPropertyChanged("Button2Weight");
            }
        }
        public FontWeight Button3Weight
        {
            get
            {
                return _b3w;
            }
            set
            {
                _b3w = value;
                if (_b3w == FontWeights.DemiBold)
                {
                    Button1Weight = FontWeights.Normal;
                    Button2Weight = FontWeights.Normal;
                    Button4Weight = FontWeights.Normal;

                    ReportsView.Filter = delegate(object item)
                    {
                        return (item as ReportInfoInt).Type == ReportType.Configured;
                    };
                    ReportsView.Refresh();
                    ReportsView.MoveCurrentToFirst();
                }
                OnPropertyChanged("Button3Weight");
            }
        }
        public FontWeight Button4Weight
        {
            get
            {
                return _b4w;
            }
            set
            {
                _b4w = value;
                if (_b4w == FontWeights.DemiBold)
                {
                    Button1Weight = FontWeights.Normal;
                    Button2Weight = FontWeights.Normal;
                    Button3Weight = FontWeights.Normal;

                    ReportsView.Filter = delegate(object item)
                    {
                        return (item as ReportInfoInt).Type == ReportType.ConfiguredParams;
                    };
                    ReportsView.Refresh();
                    ReportsView.MoveCurrentToFirst();
                }
                OnPropertyChanged("Button4Weight");
            }
        }

        public ReportLargeViewModel(IUnityContainer container)
            : base()
        {
            ReportsView = CollectionViewSource.GetDefaultView(_reports);
            Button1Weight = FontWeights.DemiBold;
        }

        protected override void RefreshDataInternal()
        {
            _reports.Clear();
            _reports.AddRange(ClientContext.GetUserReportsList());
        }

        protected override void RefreshFinished()
        {
            base.RefreshFinished();
            ReportsView.Refresh();
        }
    }
}
