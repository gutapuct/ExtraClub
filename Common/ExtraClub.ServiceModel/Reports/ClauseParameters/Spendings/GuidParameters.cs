﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.Spendings
{
    [DataContract]
    [ClauseRelation(typeof(Spending))]
    [Description("Франчайзи")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterSp1 : ClauseGuidParameter<Spending>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetCompanies().ToDictionary(j => (object)j.CompanyId, j => j.CompanyName));
            }
        }

        protected override Guid? GuidFunction(Spending entity)
        {
            return entity.CompanyId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Spending entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Spending))]
    [Description("Клуб")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterSp2 : ClauseGuidParameter<Spending>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetDivisions().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Spending entity)
        {
            return entity.DivisionId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Spending entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Spending))]
    [Description("Создатель")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameterSp3 : ClauseGuidParameter<Spending>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get { return (context) => context.GetUsers().ToDictionary(i => (object)i.UserId, i => i.FullName); }
        }

        protected override Guid? GuidFunction(Spending entity)
        {
            return entity.AuthorId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Spending entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Spending))]
    [Description("Тип")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameterSp4 : ClauseGuidParameter<Spending>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get { return (context) => context.GetDivisionSpendingTypes(Guid.Empty).ToDictionary(i => (object)i.Id, i => i.Name); }
        }

        protected override Guid? GuidFunction(Spending entity)
        {
            return entity.SpendingTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Spending entity)
        {
            throw new NotImplementedException();
        }
    }


}