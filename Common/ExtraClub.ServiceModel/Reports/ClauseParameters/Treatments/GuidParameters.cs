using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.Treatments
{
    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Тип")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameterT1 : ClauseGuidParameter<Treatment>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetAllTreatmentTypes().Where(j => j.IsActive).ToList().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Treatment entity)
        {
            return entity.TreatmentTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Treatment entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Франчайзи")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterT2 : ClauseGuidParameter<Treatment>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetCompanies().ToDictionary(j => (object)j.CompanyId, j => j.CompanyName));
            }
        }

        protected override Guid? GuidFunction(Treatment entity)
        {
            return entity.CompanyId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Treatment entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Клуб")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterT3 : ClauseGuidParameter<Treatment>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetDivisions().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Treatment entity)
        {
            return entity.DivisionId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Treatment entity)
        {
            throw new NotImplementedException();
        }
    }

}
