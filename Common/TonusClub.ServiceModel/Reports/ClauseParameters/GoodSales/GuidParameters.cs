using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.GoodSales
{
    [DataContract]
    [ClauseRelation(typeof(GoodSale))]
    [Description("Франчайзи")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterGS1 : ClauseGuidParameter<GoodSale>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetCompanies().ToDictionary(j => (object)j.CompanyId, j => j.CompanyName));
            }
        }

        protected override Guid? GuidFunction(GoodSale entity)
        {
            return entity.CompanyId;
        }

        protected override IEnumerable<Guid> GuidsFunction(GoodSale entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(GoodSale))]
#if BEAUTINIKA
    [Description("Студия")]
#else
    [Description("Клуб")]
#endif
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterGS2 : ClauseGuidParameter<GoodSale>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetDivisions().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(GoodSale entity)
        {
            return entity.BarOrder.DivisionId;
        }

        protected override IEnumerable<Guid> GuidsFunction(GoodSale entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(GoodSale))]
    [Description("Товар")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterGS3 : ClauseGuidParameter<GoodSale>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetAllGoods(Guid.Empty).ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(GoodSale entity)
        {
            return entity.GoodId;
        }

        protected override IEnumerable<Guid> GuidsFunction(GoodSale entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(GoodSale))]
    [Description("Тип карты")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameterGS4 : ClauseGuidParameter<GoodSale>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetAllCustomerCardTypes().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(GoodSale entity)
        {
            entity.BarOrder.Customer.InitActiveCard();
            if (entity.BarOrder.Customer.ActiveCard == null) return null;
            return entity.BarOrder.Customer.ActiveCard.Id;
        }

        protected override IEnumerable<Guid> GuidsFunction(GoodSale entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(GoodSale))]
    [Description("Статусы")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    public class GuidParameterGS5 : ClauseGuidParameter<GoodSale>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(c => c.GetAllStatuses().ToDictionary(i => (object)i.Key, i => i.Value));
            }
        }

        protected override Guid? GuidFunction(GoodSale entity)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Guid> GuidsFunction(GoodSale entity)
        {
            return entity.BarOrder.Customer.CustomerStatuses.Select(i => i.Id);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(GoodSale))]
    [Description("Тип оплаты")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterGS6 : ClauseGuidParameter<GoodSale>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(_ =>
                {
                    var res = new Dictionary<object, string>();
                    res.Add(new Guid(0, 0, 0, new byte[8]), "Наличные");
                    res.Add(new Guid(0, 1, 0, new byte[8]), "Депозит");
                    res.Add(new Guid(0, 2, 0, new byte[8]), "Банк. карта");
                    res.Add(new Guid(0, 3, 0, new byte[8]), "Безнал");
                    res.Add(new Guid(0, 4, 0, new byte[8]), "Бонусы");
                    return res;
                });
            }
        }

        protected override Guid? GuidFunction(GoodSale entity)
        {
            return new Guid(0, (short)entity.BarOrder.PmtTypeId, 0, new byte[8]);
        }

        protected override IEnumerable<Guid> GuidsFunction(GoodSale entity)
        {
            throw new NotImplementedException();
        }
    }

}
