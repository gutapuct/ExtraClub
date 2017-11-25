using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.Goods
{
    [DataContract]
    [ClauseRelation(typeof(Good))]
    [Description("Франчайзи")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterGo1 : ClauseGuidParameter<Good>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetCompanies().ToDictionary(j => (object)j.CompanyId, j => j.CompanyName));
            }
        }

        protected override Guid? GuidFunction(Good entity)
        {
            return entity.CompanyId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Good entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Good))]
    [Description("Тип продукта")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    public class GuidParameterGo2 : ClauseGuidParameter<Good>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetDictionaryList("ProductTypes").ToDictionary(j => (object)j.Key, j => j.Value));
            }
        }

        protected override Guid? GuidFunction(Good entity)
        {
            return entity.ProductTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Good entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Good))]
    [Description("Производитель")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    public class GuidParameterGo3 : ClauseGuidParameter<Good>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetDictionaryList("Manufacturers").ToDictionary(j => (object)j.Key, j => j.Value));
            }
        }

        protected override Guid? GuidFunction(Good entity)
        {
            return entity.ManufacturerId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Good entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Good))]
    [Description("Ед. измерения")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    public class GuidParameterGo4 : ClauseGuidParameter<Good>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetDictionaryList("UnitTypes").ToDictionary(j => (object)j.Key, j => j.Value));
            }
        }

        protected override Guid? GuidFunction(Good entity)
        {
            return entity.UnitTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Good entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Good))]
    [Description("Категория")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    public class GuidParameterGo5 : ClauseGuidParameter<Good>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetDictionaryList("GoodsCategories").ToDictionary(j => (object)j.Key, j => j.Value));
            }
        }

        protected override Guid? GuidFunction(Good entity)
        {
            return entity.GoodsCategoryId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Good entity)
        {
            throw new NotImplementedException();
        }
    }

}
