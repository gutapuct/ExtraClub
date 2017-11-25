using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ReportColumns
{
    [ReportColumns(typeof(Employee))]
    public class EmployeeColumns
    {
        Employee entity { get; set; }
        public EmployeeColumns(Employee obj)
        {
            entity = obj;
        }
        public Guid Id { get { return entity.Id; } }

        [Description("Франчайзи")]
        public string f1 { get { return entity.Company.CompanyName; } }
#if BEAUTINIKA
        [Description("Студия")]
#else
    [Description("Клуб")]
#endif
        public string f2 { get { return entity.MainDivision.Name; } }
        [Description("Создатель")]
        public string f3 { get { return entity.CreatedBy.FullName; } }
        [Description("ФИО")]
        public string f4 { get { return entity.BoundCustomer.FullName; } }
        [Description("Катра")]
        public string f5
        {
            get
            {
                entity.Init();
                return entity.SerializedCardNumber;
            }
        }
        [Description("Должность")]
        public string f6
        {
            get
            {
                entity.Init();
                if (entity.SerializedJobPlacement == null) return null;
                return entity.SerializedJobPlacement.Job.Name;
            }
        }
        [Description("Схема премий")]
        public string f7
        {
            get
            {
                entity.Init();
                if (entity.SerializedJobPlacement == null) return null;
                if (!entity.SerializedJobPlacement.Job.SalarySchemeId.HasValue) return null;
                return entity.SerializedJobPlacement.Job.SalaryScheme.Name;
            }
        }
        [Description("Подразделение")]
        public string f8
        {
            get
            {
                entity.Init();
                if (entity.SerializedJobPlacement == null) return null;
                return entity.SerializedJobPlacement.Job.Unit;
            }
        }
        [Description("Оклад")]
        public decimal? f9
        {
            get
            {
                entity.Init();
                if (entity.SerializedJobPlacement == null) return null;
                return entity.SerializedJobPlacement.Salary;
            }
        }
        [Description("Схема работы")]
        public string f10
        {
            get
            {
                entity.Init();
                if (entity.SerializedJobPlacement == null) return null;
                return entity.SerializedJobPlacement.Job.WorkGraph;
            }
        }
        [Description("В отпуске ли")]
        public string f11
        {
            get
            {
                return entity.EmployeeVacations.Any(i => i.VacationType == 0 && i.BeginDate <= DateTime.Today && i.EndDate >= DateTime.Today) ? "Да" : "Нет";
            }
        }
        [Description("На больничном ли")]
        public string f12
        {
            get
            {
                return entity.EmployeeVacations.Any(i => i.VacationType == 1 && i.BeginDate <= DateTime.Today && i.EndDate >= DateTime.Today) ? "Да" : "Нет";
            }
        }
        [Description("В командировке ли")]
        public string f13
        {
            get
            {
                return entity.EmployeeTrips.Any(i => i.BeginDate <= DateTime.Today && i.EndDate >= DateTime.Today) ? "Да" : "Нет";
            }
        }
        [Description("День рождения")]
        public DateTime? f14
        {
            get
            {
                return entity.BoundCustomer.Birthday;
            }
        }
    }
}
