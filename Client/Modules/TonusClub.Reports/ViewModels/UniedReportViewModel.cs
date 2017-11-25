using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using TonusClub.Infrastructure.BaseClasses;
using TonusClub.ServiceModel.Reports;
using TonusClub.UIControls.BaseClasses;

namespace TonusClub.Reports.ViewModels
{
    public class UniedReportViewModel : ViewModelBase
    {
        public List<SalesData> SalesDynamics { get; set; }
        public List<ChannelData> SalesChannels { get; set; }
        public List<ChannelData> AmountTreatments { get; set; }
        public List<SalesData> VisitsDynamics { get; set; }
        public List<SalesData> IncomeAmount { get; set; }
        public List<TicketsData> AmountTicket { get; set; }

        public Visibility LicenseVisibility { get; set; }
        public string LicenceText { get; set; }

        bool _SalesDynamicsClub = false;
        public bool SalesDynamicsClub
        {
            get
            {
                return _SalesDynamicsClub;
            }
            set
            {
                _SalesDynamicsClub = value;
                if (value)
                {
                    SalesDynamics = ClientContext.GetUniedReportTicketSalesDynamic(true);
                    OnPropertyChanged("SalesDynamicsClub");
                    OnPropertyChanged("SalesDynamicsFranch");
                    OnPropertyChanged("SalesDynamics");
                }
            }
        }

        bool _SalesDynamicsFranch = true;
        public bool SalesDynamicsFranch
        {
            get
            {
                return _SalesDynamicsFranch;
            }
            set
            {
                _SalesDynamicsFranch = value;
                if (value)
                {
                    SalesDynamics = ClientContext.GetUniedReportTicketSalesDynamic(false);
                    OnPropertyChanged("SalesDynamicsClub");
                    OnPropertyChanged("SalesDynamicsFranch");
                    OnPropertyChanged("SalesDynamics");
                }
            }
        }


        bool _IncomeAmountClub = false;
        public bool IncomeAmountClub
        {
            get
            {
                return _IncomeAmountClub;
            }
            set
            {
                _IncomeAmountClub = value;
                if (value)
                {

                    IncomeAmount = ClientContext.GetUniedReportIncomeAmount(true);
                    OnPropertyChanged("IncomeAmount");
                    OnPropertyChanged("IncomeAmountClub");
                    OnPropertyChanged("IncomeAmountFranch");
                }
            }
        }

        bool _IncomeAmountFranch = true;
        public bool IncomeAmountFranch
        {
            get
            {
                return _IncomeAmountFranch;
            }
            set
            {
                _IncomeAmountFranch = value;
                if (value)
                {

                    IncomeAmount = ClientContext.GetUniedReportIncomeAmount(false);
                    OnPropertyChanged("IncomeAmount");
                    OnPropertyChanged("IncomeAmountClub");
                    OnPropertyChanged("IncomeAmountFranch");
                }
            }
        }

        bool _AmountTicketClub = true;
        public bool AmountTicketClub
        {
            get
            {
                return _AmountTicketClub;
            }
            set
            {
                _AmountTicketClub = value;
                if (value)
                {

                    AmountTicket = ClientContext.GetUniedReportAmountTicket(true);
                    OnPropertyChanged("AmountTicket");
                    OnPropertyChanged("AmountTicketClub");
                    OnPropertyChanged("AmountTicketFranch");
                }
            }
        }

        bool _AmountTicketFranch = false;
        public bool AmountTicketFranch
        {
            get
            {
                return _AmountTicketFranch;
            }
            set
            {
                _AmountTicketFranch = value;
                if (value)
                {

                    AmountTicket = ClientContext.GetUniedReportAmountTicket(false);
                    OnPropertyChanged("AmountTicket");
                    OnPropertyChanged("AmountTicketClub");
                    OnPropertyChanged("AmountTicketFranch");
                }
            }
        }

        bool _SalesChannels365 = true;
        public bool SalesChannels365
        {
            get
            {
                return _SalesChannels365;
            }
            set
            {
                _SalesChannels365 = value;
                if (value)
                {
                    SalesChannels = ClientContext.GetUniedReportSalesChannels(365);
                    OnPropertyChanged("SalesChannels");
                    OnPropertyChanged("SalesChannels365");
                    OnPropertyChanged("SalesChannels30");
                    OnPropertyChanged("SalesChannels7");
                }
            }
        }

        bool _SalesChannels30;
        public bool SalesChannels30
        {
            get
            {
                return _SalesChannels30;
            }
            set
            {
                _SalesChannels30 = value;
                if (value)
                {
                    SalesChannels = ClientContext.GetUniedReportSalesChannels(30);
                    OnPropertyChanged("SalesChannels");
                    OnPropertyChanged("SalesChannels365");
                    OnPropertyChanged("SalesChannels30");
                    OnPropertyChanged("SalesChannels7");
                }
            }
        }

        bool _SalesChannels7;
        public bool SalesChannels7
        {
            get
            {
                return _SalesChannels7;
            }
            set
            {
                _SalesChannels7 = value;
                if (value)
                {
                    SalesChannels = ClientContext.GetUniedReportSalesChannels(7);
                    OnPropertyChanged("SalesChannels");
                    OnPropertyChanged("SalesChannels365");
                    OnPropertyChanged("SalesChannels30");
                    OnPropertyChanged("SalesChannels7");
                }
            }
        }

        bool _AmountTreatments365;
        public bool AmountTreatments365
        {
            get
            {
                return _AmountTreatments365;
            }
            set
            {
                _AmountTreatments365 = value;
                if (value)
                {
                    AmountTreatments = ClientContext.GetUniedReportAmountTreatments(365);
                    OnPropertyChanged("AmountTreatments");
                    OnPropertyChanged("AmountTreatments365");
                    OnPropertyChanged("AmountTreatments90");
                    OnPropertyChanged("AmountTreatments30");
                }
            }
        }

        bool _AmountTreatments90;
        public bool AmountTreatments90
        {
            get
            {
                return _AmountTreatments90;
            }
            set
            {
                _AmountTreatments90 = value;
                if (value)
                {
                    AmountTreatments = ClientContext.GetUniedReportAmountTreatments(90);
                    OnPropertyChanged("AmountTreatments");
                    OnPropertyChanged("AmountTreatments365");
                    OnPropertyChanged("AmountTreatments90");
                    OnPropertyChanged("AmountTreatments30");
                }
            }
        }

        bool _AmountTreatments30 = true;
        public bool AmountTreatments30
        {
            get
            {
                return _AmountTreatments30;
            }
            set
            {
                _AmountTreatments30 = value;
                if (value)
                {
                    AmountTreatments = ClientContext.GetUniedReportAmountTreatments(30);
                    OnPropertyChanged("AmountTreatments");
                    OnPropertyChanged("AmountTreatments365");
                    OnPropertyChanged("AmountTreatments90");
                    OnPropertyChanged("AmountTreatments30");
                }
            }
        }

        bool _VisitsDynamicsClients = true;
        public bool VisitsDynamicsClients
        {
            get
            {
                return _VisitsDynamicsClients;
            }
            set
            {
                _VisitsDynamicsClients = value;
                if (value)
                {
                    VisitsDynamics = ClientContext.GetUniedReportVisitsDynamics(0);
                    OnPropertyChanged("VisitsDynamics");
                    OnPropertyChanged("VisitsDynamicsClients");
                    OnPropertyChanged("VisitsDynamicsEvents");
                    OnPropertyChanged("VisitsDynamicsCharges");
                }
            }
        }

        bool _VisitsDynamicsEvents;
        public bool VisitsDynamicsEvents
        {
            get
            {
                return _VisitsDynamicsEvents;
            }
            set
            {
                _VisitsDynamicsEvents = value;
                if (value)
                {
                    VisitsDynamics = ClientContext.GetUniedReportVisitsDynamics(1);
                    OnPropertyChanged("VisitsDynamics");
                    OnPropertyChanged("VisitsDynamicsClients");
                    OnPropertyChanged("VisitsDynamicsEvents");
                    OnPropertyChanged("VisitsDynamicsCharges");
                }
            }
        }

        bool _VisitsDynamicsCharges;
        public bool VisitsDynamicsCharges
        {
            get
            {
                return _VisitsDynamicsCharges;
            }
            set
            {
                _VisitsDynamicsCharges = value;
                if (value)
                {
                    VisitsDynamics = ClientContext.GetUniedReportVisitsDynamics(2);
                    OnPropertyChanged("VisitsDynamics");
                    OnPropertyChanged("VisitsDynamicsClients");
                    OnPropertyChanged("VisitsDynamicsEvents");
                    OnPropertyChanged("VisitsDynamicsCharges");
                }
            }
        }
        
        public UniedReportViewModel(IUnityContainer container)
            : base()
        {
            LicenseVisibility = Visibility.Collapsed;
        }

        protected override void RefreshDataInternal()
        {
            SalesDynamics = ClientContext.GetUniedReportTicketSalesDynamic(_SalesDynamicsClub);
            OnPropertyChanged("SalesDynamics");

            SalesChannels = ClientContext.GetUniedReportSalesChannels(365);
            OnPropertyChanged("SalesChannels");

            AmountTreatments = ClientContext.GetUniedReportAmountTreatments(30);
            OnPropertyChanged("AmountTreatments");

            VisitsDynamics = ClientContext.GetUniedReportVisitsDynamics(0);
            OnPropertyChanged("VisitsDynamics");

            IncomeAmount = ClientContext.GetUniedReportIncomeAmount(_IncomeAmountClub);
            OnPropertyChanged("IncomeAmount");

            AmountTicket = ClientContext.GetUniedReportAmountTicket(_AmountTicketClub);
            OnPropertyChanged("AmountTicket");

            //var ls = ClientContext.GetLocalSettings();
            //if (ls != null)
            //{
            //    var lic = ls.LicenseExpiry;
            //    if (lic.HasValue && DateTime.Today.AddDays(7) >= lic)
            //    {
            //        LicenceText = String.Format("Лицензия на использование Flagmax Direction истекает {0:d MMMM yyyy}", lic.Value);
            //        LicenseVisibility = Visibility.Visible;
            //        OnPropertyChanged("LicenceText");
            //    }
            //}
            //else
            {
                LicenseVisibility = Visibility.Collapsed;
            }
            OnPropertyChanged("LicenseVisibility");

        }

    }
}
