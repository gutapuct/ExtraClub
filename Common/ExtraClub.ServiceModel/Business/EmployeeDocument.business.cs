using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class EmployeeDocument : IInitable
    {
        [DataMember]
        public Guid DivisionId { get; set; }

        [DataMember]
        public string SerializedEmployeeName { get; set; }

        [DataMember]
        public string SerializedJobName { get; set; }

        [DataMember]
        public string SerializedUnit { get; set; }

        [DataMember]
        public string SerializedStatusText { get; set; }

        [DataMember]
        public Guid? ReportParameter { get; set; }

        public Employee Employee { get; set; }

        public void Init()
        {
            if (EmployeeTrips.Count > 0)
            {
                Employee = EmployeeTrips.First().Employee;
                ReportParameter = EmployeeTrips.First().Id;
            }
            else if (EmployeeVacations.Count > 0)
            {
                var vac = EmployeeVacations.First();
                Employee = vac.Employee;
                if (vac.VacationType == 0)
                {
                    ReportParameter = vac.Id;
                }
            }
            else if (JobPlacements.Count > 0)
            {
                Employee = JobPlacements.Select(i => i.Employee).FirstOrDefault();
                if (!JobPlacements.First().IsAsset)
                    SerializedStatusText = "Сохранен";
                ReportParameter = JobPlacements.Select(i => i.Id).FirstOrDefault();
            }
            else
            {
                Employee = JobPlacements1.Select(i=>i.Employee).FirstOrDefault();
                ReportParameter = JobPlacements1.Select(i => i.Id).FirstOrDefault();
            }

            if (String.IsNullOrEmpty(SerializedStatusText))
            {
                SerializedStatusText = "Проведен";
            }
            if (Employee != null)
            {
                DivisionId = Employee.MainDivisionId;
                Employee.Init();
                SerializedEmployeeName = Employee.BoundCustomer.FullName;
                if (Employee.SerializedJobPlacement != null)
                {
                    SerializedJobName = Employee.SerializedJobPlacement.Job.Name;
                    SerializedUnit = Employee.SerializedJobPlacement.Job.Unit;
                }
            }
        }

        public string DocTypeText
        {
            get
            {
                switch (DocType)
                {
                    case (short)DocumentTypes.CategoryChange:
                        return "Смена категории";
                    case (short)DocumentTypes.Ill:
                        return "Больничный";
                    case (short)DocumentTypes.JobApply:
                        return "Прием на работу";
                    case (short)DocumentTypes.JobChange:
                        return "Перевод на другую должность";
                    case (short)DocumentTypes.JobFire:
                        return "Увольнение";
                    case (short)DocumentTypes.Miss:
                        return "Отгул";
                    case (short)DocumentTypes.Trip:
                        return "Командировка";
                    case (short)DocumentTypes.Vacation:
                        return "Отпуск";
                }
                return "Прочее";
            }
        }
    }
}
