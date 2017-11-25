using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure.Interfaces;
using Microsoft.Practices.Composite.Regions;
using System.Configuration;
using System.Windows.Data;
using System.ComponentModel;
using TonusClub.ServiceModel;
using System.Collections.ObjectModel;
using System.Collections;
using TonusClub.Infrastructure.BaseClasses;
using System.ServiceModel;
using TonusClub.UIControls.BaseClasses;

namespace TonusClub.TurnoverModule.ViewModels
{
    public class ConsignmentViewModel : ViewModelBase
    {
        public List<Provider> Providers { get; set; }


        public ObservableCollection<ConsignmentLine> ConsignmentLines { get; set; }

        public ICollectionView ConsignmentLinesView { get; private set; }
        public ObservableCollection<Good> Goods { get; set; }
        public List<Storehouse> Storehouses { get; set; }

        public Consignment Consignment { get; set; }

        public User CurrentUser { get; set; }

        //public Provider SelectedProvider { get; set; }
        //public DictionaryPair SelectedDivision { get; set; }

        List<Good> goods;

        public ConsignmentViewModel(IUnityContainer container, Consignment consignment, int type)
            : base()
        {
            Goods = new ObservableCollection<Good>();
            goods = ClientContext.GetAllGoods();
            goods.ForEach(i => Goods.Add(i));
            Storehouses = ClientContext.GetStorehouses().Where(i => i.IsActive).ToList();
            CurrentUser = ClientContext.CurrentUser;
            if (consignment.Id != Guid.Empty)
            {
                Consignment = consignment;
                ConsignmentLines = new ObservableCollection<ConsignmentLine>();
                consignment.SerializedConsignmentLines.ForEach(i => ConsignmentLines.Add(i));
            }
            else
            {
                Consignment = new Consignment { Date = DateTime.Today, DivisionId = ClientContext.CurrentDivision.Id, Id = Guid.NewGuid(), DocType = (short)type, IsAsset = false };

                ConsignmentLines = new FixupCollection<ConsignmentLine>();
                ConsignmentLines.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ConsignmentLines_CollectionChanged);
            }
            Providers = ClientContext.GetAllProviders();
            ConsignmentLinesView = CollectionViewSource.GetDefaultView(ConsignmentLines);

            Consignment.PropertyChanged += new PropertyChangedEventHandler(Consignment_PropertyChanged);
        }

        void Consignment_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SourceStorehouseId" && Consignment.DocType == 1)
            {
                Goods.Clear();

                ClientContext.GetGoodsPresence().Where(i => i.StorehouseId == Consignment.SourceStorehouseId).ToList().ForEach(i =>
                {
                    var g = goods.SingleOrDefault(j => j.Id == i.GoodId);
                    if (g != null) Goods.Add(g);
                });
            }
        }

        void ConsignmentLines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null) return;
            if (e.NewItems.Count == 1 && !((ConsignmentLine)(e.NewItems[0])).Position.HasValue)
            {
                ((ConsignmentLine)(e.NewItems[0])).Position = (short)ConsignmentLines.Count;
            }
        }

        internal Consignment PostConsignment()
        {
            if ((!Consignment.DestinationStorehouseId.HasValue || Consignment.DestinationStorehouseId == Guid.Empty) && (Consignment.DocType == 0 || Consignment.DocType == 1 || Consignment.DocType == 3))
            {
                throw new NullReferenceException(UIControls.Localization.Resources.NoDestination);
            }

            if ((!Consignment.ProviderId.HasValue || Consignment.ProviderId == Guid.Empty) && (Consignment.DocType == 0 || Consignment.DocType == 3))
            {
                throw new NullReferenceException(UIControls.Localization.Resources.NoSource);
            }

            if ((!Consignment.SourceStorehouseId.HasValue || Consignment.SourceStorehouseId == Guid.Empty) && (Consignment.DocType == 1 || Consignment.DocType == 2))
            {
                throw new NullReferenceException(UIControls.Localization.Resources.NoSource);
            }
            ConsignmentLines.ToList().ForEach(l =>
                {
                    Consignment.ConsignmentLines.Add(l);
                    l.ConsignmentId = Consignment.Id;
                });

            //if (isTransfer)
            //{
            //    var prov = Providers.FirstOrDefault(p => p.Id == Consignment.ProviderId);
            //    if (prov == null) throw new Exception("Поставщик не является филиалом");

            //    var pres = _context.GetGoodsPresence(prov.DivisionId);
            //    foreach (var l in ConsignmentLines)
            //    {
            //        var goodInfo = pres.FirstOrDefault(g => g.GoodId == l.GoodId);
            //        if (goodInfo == null)
            //        {
            //            var good = Goods.First(g => g.Id == l.GoodId);
            //            throw new ArgumentException(good.Name);
            //        }
            //        if (goodInfo.Amount < l.Quantity) throw new ArgumentException(goodInfo.Name);
            //    }
            //}

            if (Consignment.DocType == 2 || Consignment.DocType == 1)
            {
                var gp = ClientContext.GetGoodsPresence();
                var left = gp.Where(i => i.StorehouseId == Consignment.SourceStorehouseId).GroupBy(i => i.GoodId).ToDictionary(i => i.Key, i => i.Sum(j => j.Amount));
                foreach (var line in Consignment.ConsignmentLines)
                {
                    if (!left.ContainsKey(line.GoodId) || left[line.GoodId] < line.Quantity)
                    {
                        throw new FaultException<string>("", "На складе недостаточно товара " + goods.Where(j => j.Id == line.GoodId).Select(j => j.Name).FirstOrDefault());
                    }
                }
            }

            ClientContext.PostConsignment(Consignment);
            ClientContext.PostConsignmentLines(Consignment.ConsignmentLines);

            return Consignment;
        }


        protected override void RefreshDataInternal()
        {
        }

        internal void AssetConsignment()
        {
            Consignment.IsAsset = true;
        }

        internal void SetRashod(bool rashod)
        {
            if (!Consignment.IsAsset)
            {
#if BEAUTINIKA
                Consignment.IsMaterialWriteoff = rashod;
#endif
            }
        }
    }
}
