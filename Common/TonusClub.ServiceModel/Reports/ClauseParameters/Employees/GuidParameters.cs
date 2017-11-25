using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.Employees
{
    [DataContract]
    [ClauseRelation(typeof(Employee))]
    [Description("Франчайзи")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterEm1 : ClauseGuidParameter<Employee>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetCompanies().ToDictionary(j => (object)j.CompanyId, j => j.CompanyName));
            }
        }

        protected override Guid? GuidFunction(Employee entity)
        {
            return entity.CompanyId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Employee entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Employee))]
#if BEAUTINIKA
    [Description("Студия")]
#else
    [Description("Клуб")]
#endif
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterEm2 : ClauseGuidParameter<Employee>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetDivisions().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Employee entity)
        {
            return entity.MainDivisionId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Employee entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Employee))]
    [Description("Создатель")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameterEm3 : ClauseGuidParameter<Employee>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get { return (context) => context.GetUsers().ToDictionary(i => (object)i.UserId, i => i.FullName); }
        }

        protected override Guid? GuidFunction(Employee entity)
        {
            return entity.AuthorId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Employee entity)
        {
            throw new NotImplementedException();
        }
    }


}
