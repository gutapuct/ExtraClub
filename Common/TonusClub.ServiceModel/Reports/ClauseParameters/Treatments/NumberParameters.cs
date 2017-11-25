using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.Treatments
{
    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Максимум посетителей")]
    public class NumberParameterT1 : ClauseNumberParameter<Treatment>
    {
        protected override decimal? NumberFunction(Treatment entity)
        {
            return entity.MaxCustomers;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Число процедур всего")]
    public class NumberParameterT2 : ClauseNumberParameter<Treatment>
    {
        protected override decimal? NumberFunction(Treatment entity)
        {
            return entity.TreatmentEvents.Where(i => i.VisitStatus == 2).Count();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Число процедур за последний год")]
    public class NumberParameterT3 : ClauseNumberParameter<Treatment>
    {
        protected override decimal? NumberFunction(Treatment entity)
        {
            var date = DateTime.Today.AddYears(-1);
            return entity.TreatmentEvents.Where(i => i.VisitStatus == 2 && i.VisitDate >= date).Count();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Среднесуточная загрузка")]
    public class NumberParameterT4 : ClauseNumberParameter<Treatment>
    {
        protected override decimal? NumberFunction(Treatment entity)
        {
            var procs = (decimal)entity.TreatmentEvents.Where(i => i.VisitStatus == 2).Count();
            var days = (decimal)(DateTime.Today - entity.CreatedOn).TotalDays;
            if (days < 1) return 0;
            return procs / days;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Среднесуточная загрузка за год")]
    public class NumberParameterT5 : ClauseNumberParameter<Treatment>
    {
        protected override decimal? NumberFunction(Treatment entity)
        {
            var date = DateTime.Today.AddYears(-1);
            var procs = (decimal)entity.TreatmentEvents.Where(i => i.VisitStatus == 2 && i.VisitDate >= date).Count();
            var days = (decimal)(DateTime.Today - entity.CreatedOn).TotalDays;
            if (days > 365) days = 365;
            if (days < 1) return 0;
            return procs / days;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Средненедельная загрузка")]
    public class NumberParameterT6 : ClauseNumberParameter<Treatment>
    {
        protected override decimal? NumberFunction(Treatment entity)
        {
            var procs = (decimal)entity.TreatmentEvents.Where(i => i.VisitStatus == 2).Count();
            var weeks = (decimal)(DateTime.Today - entity.CreatedOn).TotalDays/7;
            if (weeks < 1) return 0;
            return procs / weeks;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Средненедельная загрузка за 364 дня")]
    public class NumberParameterT7 : ClauseNumberParameter<Treatment>
    {
        protected override decimal? NumberFunction(Treatment entity)
        {
            var date = DateTime.Today.AddDays(-364);
            var procs = (decimal)entity.TreatmentEvents.Where(i => i.VisitStatus == 2 && i.VisitDate >= date).Count();
            var weeks = (decimal)(DateTime.Today - entity.CreatedOn).TotalDays/7;
            if (weeks > 52) weeks = 52;
            if (weeks < 1) return 0;
            return procs / weeks;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Среднемесячная загрузка")]
    public class NumberParameterT8 : ClauseNumberParameter<Treatment>
    {
        protected override decimal? NumberFunction(Treatment entity)
        {
            var procs = (decimal)entity.TreatmentEvents.Where(i => i.VisitStatus == 2).Count();
            var months = (decimal)(DateTime.Today - entity.CreatedOn).TotalDays / 30.5m;
            if (months < 1) return 0;
            return procs / months;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Среднемесячная загрузка за год")]
    public class NumberParameterT9 : ClauseNumberParameter<Treatment>
    {
        protected override decimal? NumberFunction(Treatment entity)
        {
            var date = DateTime.Today.AddYears(-1);
            var procs = (decimal)entity.TreatmentEvents.Where(i => i.VisitStatus == 2 && i.VisitDate >= date).Count();
            var months = (decimal)(DateTime.Today - entity.CreatedOn).TotalDays / 30.5m;
            if (months > 12) months = 12;
            if (months < 1) return 0;
            return procs / months;
        }
    }

}
