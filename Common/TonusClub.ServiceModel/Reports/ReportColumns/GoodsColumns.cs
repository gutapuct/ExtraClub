using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ReportColumns
{
    [ReportColumns(typeof(Good))]
    public class GoodsColumns
    {
        Good entity { get; set; }
        public GoodsColumns(Good obj)
        {
            entity = obj;
        }
        public Guid Id { get { return entity.Id; } }

        [Description("Наименование")]
        public string f1 { get { return entity.Name; } }
        [Description("Франчайзи")]
        public string f2 { get { return entity.Company.CompanyName; } }
        [Description("Ед. изм.")]
        public string f3 { get { return entity.UnitTypeId.HasValue ? entity.UnitType.Name : (string)null; } }
        [Description("Тип продукта")]
        public string f4 { get { return entity.ProductTypeId.HasValue ? entity.ProductType.Name : (string)null; } }
        [Description("Производитель")]
        public string f5 { get { return entity.ManufacturerId.HasValue ? entity.Manufacturer.Name : (string)null; } }
        [Description("Категория")]
        public string f6 { get { return entity.GoodsCategoryId.HasValue ? entity.GoodsCategory.Name : (string)null; } }
        [Description("Продаж всего")]
        public int f7
        {
            get
            {
                if (!entity.GoodSales.Any()) return 0;
                return (int)entity.GoodSales.Sum(i => i.Amount);
            }
        }
        [Description("Поставок всего")]
        public int f8
        {
            get
            {
                if (!entity.ConsignmentLines.Any(i => i.Consignment.ProviderId.HasValue && i.Consignment.IsAsset)) return 0;
                return (int)entity.ConsignmentLines.Where(i => i.Consignment.ProviderId.HasValue && i.Consignment.IsAsset).Sum(i => i.Quantity);
            }
        }
        [Description("Списаний всего")]
        public int f9
        {
            get
            {
                if (!entity.ConsignmentLines.Any(i => !i.Consignment.ProviderId.HasValue && !i.Consignment.DestinationStorehouseId.HasValue && i.Consignment.IsAsset)) return 0;
                return (int)entity.ConsignmentLines.Where(i => !i.Consignment.ProviderId.HasValue && !i.Consignment.DestinationStorehouseId.HasValue && i.Consignment.IsAsset).Sum(i => i.Quantity);
            }
        }
        [Description("Остаток на всех складах")]
        public int f10 { get { return (int)entity.TotalLeft; } }

        [Description("Остаток на каждом складе")]
        public string f11 { get { return entity.LeftByDiv; } }

        [Description("Цены")]
        public string f12 { get { return entity.PriceByDiv; } }
        [Description("Максимальная цена")]
        public decimal? f13 { get { return entity.MaxPrice; } }
        [Description("Минимальная цена")]
        public decimal? f14 { get { return entity.MinPrice; } }
    }
}
