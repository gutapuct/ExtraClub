using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ReportColumns
{
    [ReportColumns(typeof(CustomerTarget))]
    public class CustomerTargetColumns
    {
        public CustomerTarget Value {get;set;}
        public CustomerTargetColumns(CustomerTarget value){
            Value = value;
        }
        public Guid Id { get { return Value.Id; } }

        [Description("Дата постановки цели")]
        public DateTime CreatedOn
        {
            get
            {
                return Value.CreatedOn.Date;
            }
        }

        [Description("Время постановки цели")]
        public string CreatedOnTime
        {
            get
            {
                return Value.CreatedOn.ToString("HH:mm");
            }
        }

        [Description("Дата достижения цели")]
        public DateTime CompletedOn
        {
            get
            {
                return Value.TargetDate.Date;
            }
        }

        [Description("Время достижения цели")]
        public string CompletedOnTime
        {
            get
            {
                return Value.TargetDate.ToString("HH:mm");
            }
        }

        [Description("Сотрудник")]
        public string EmployeeName
        {
            get
            {
                return Value.CreatedBy.FullName;
            }
        }

        [Description("Клиент")]
        public string CustomerName
        {
            get
            {
                return Value.Customer.ShortName;
            }
        }

        [Description("Карта")]
        public string CustomerCard
        {
            get
            {
                Value.Customer.InitActiveCard();
                if (Value.Customer.ActiveCard == null) return String.Empty;
                return Value.Customer.ActiveCard.CardBarcode;
            }
        }

        [Description("Цель")]
        public string TargetText
        {
            get
            {
                return Value.TargetText;
            }
        }

        [Description("Тип цели")]
        public string TargetType
        {
            get
            {
                return Value.CustomerTargetType.Name;
            }
        }

        [Description("Наступила дата")]
        public string IsReached
        {
            get
            {
                return Value.TargetDate < DateTime.Now ? "Да" : "Нет";
            }
        }

        [Description("Цель достигнута")]
        public string IsComlpeted
        {
            get
            {
                if (!Value.TargetComplete.HasValue) return "";
                return Value.TargetComplete.Value ? "Да" : "Нет";
            }
        }
    }

}
