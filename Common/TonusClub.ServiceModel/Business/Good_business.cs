using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class Good : IInitable
    {
        public bool IsSelected { get; set; }

        /// <summary>
        /// Internal-use
        /// </summary>
        public int Amount { get; set; }

        [DataMember]
        public string SerializedUnitType { get; set; }
        [DataMember]
        public string SerializedManufacturer { get; set; }
        [DataMember]
        public string SerializedGoodsCategory { get; set; }
        [DataMember]
        public string SerializedProductType { get; set; }
        [DataMember]
        public string SerializedMaterialType { get; set; }

        public void Init()
        {
            if (UnitTypeId.HasValue)
            {
                SerializedUnitType = UnitType.Name;
            }
            if (ManufacturerId.HasValue)
            {
                SerializedManufacturer = Manufacturer.Name;
            }
            if (GoodsCategoryId.HasValue)
            {
                SerializedGoodsCategory = GoodsCategory.Name;
            }
            if (ProductTypeId.HasValue)
            {
                SerializedProductType = ProductType.Name;
            }
#if BEAUTINIKA
            if (MaterialUnitId.HasValue && OutMulti.HasValue && OutMulti > 0)
            {
                SerializedMaterialType = "Да";
            }
#endif
        }

        public decimal TotalLeft
        {
            get
            {
                var income = .0;
                var incomeList = ConsignmentLines.Where(l => l.Consignment.IsAsset && l.Consignment.DocType == 0);
                if (incomeList.Count() > 0)
                {
                    income = incomeList.Sum(i => i.Quantity ?? 0);
                }
                var outcome = .0;
                var outcomeList1 = ConsignmentLines.Where(l => l.Consignment.SourceStorehouseId.HasValue && l.Consignment.IsAsset && l.Consignment.DocType == 2);
                var outcomeList2 = GoodSales.Where(s => !s.ReturnById.HasValue);
                var outcomeList3 = Rents.Where(r => !r.FactReturnDate.HasValue || r.LostFine.HasValue);
                var am2 = outcomeList2.Count() > 0 ? outcomeList2.Sum(i => i.Amount) : 0;
                outcome = (outcomeList1.Count() > 0 ? outcomeList1.Sum(i => i.Quantity ?? 0) : 0) + am2 + outcomeList3.Count();
                return (decimal)(income - outcome);
            }
        }

        public decimal? MaxPrice
        {
            get
            {
                if (!GoodPrices.Any()) return null;
                return GoodPrices.Max(i => i.CommonPrice);
            }
        }


        public decimal? MinPrice
        {
            get
            {
                if (!GoodPrices.Any()) return null;
                return GoodPrices.Min(i => i.CommonPrice);
            }
        }

        public string LeftByDiv
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var store in ConsignmentLines.Select(i => i.Consignment.DestinationStorehouse).Distinct())
                {
                    if (store == null) continue;
                    var income = .0;
                    var incomeList = ConsignmentLines.Where(l => l.Consignment.DestinationStorehouseId == store.Id && l.Consignment.IsAsset && (l.Consignment.DocType == 0 || l.Consignment.DocType == 1));
                    if (incomeList.Count() > 0)
                    {
                        income = incomeList.Sum(i => i.Quantity ?? 0);
                    }
                    var outcome = .0;
                    var outcomeList1 = ConsignmentLines.Where(l => l.Consignment.SourceStorehouseId == store.Id && l.Consignment.IsAsset && (l.Consignment.DocType == 1 || l.Consignment.DocType == 2));
                    var outcomeList2 = GoodSales.Where(s => s.StorehouseId == store.Id && !s.ReturnById.HasValue);
                    var outcomeList3 = Rents.Where(r => (!r.FactReturnDate.HasValue || r.LostFine.HasValue) && r.StorehouseId == store.Id);
                    var am2 = outcomeList2.Count() > 0 ? outcomeList2.Sum(i => i.Amount) : 0;
                    outcome = (outcomeList1.Count() > 0 ? outcomeList1.Sum(i => i.Quantity ?? 0) : 0) + am2 + outcomeList3.Count();
                    if (income != outcome)
                    {
                        if (sb.Length > 0) sb.Append("; ");
                        sb.AppendFormat("{0}: {1}", store.Name, income - outcome);
                    }
                }
                return sb.ToString();
            }
        }

        public string PriceByDiv
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var div in ConsignmentLines.Where(i=>i.Consignment.DestinationStorehouseId.HasValue).Select(i => i.Consignment.DestinationStorehouse.Division).Distinct())
                {
                    if (GoodPrices.Any(i => i.DivisionId == div.Id))
                    {
                        var gp = GoodPrices.Where(i => i.DivisionId == div.Id).OrderByDescending(i=>i.Date).First();
                        if (gp.InPricelist)
                        {
                            if (sb.Length > 0) sb.Append("; ");
                            sb.AppendFormat("{0}: {1:c}", div.Name, gp.CommonPrice);
                        }
                    }
                }
                return sb.ToString();
            }
        }
    }
}