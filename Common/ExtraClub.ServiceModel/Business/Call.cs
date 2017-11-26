using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ExtraClub.ServiceModel.Organizer;

namespace ExtraClub.ServiceModel
{
    partial class Call : IInitable
    {
        [DataMember]
        public string SerializedCreatedBy { get; set; }
        [DataMember]
        public string SerializedCustomerName { get; set; }
        [DataMember]
        public string SeriazlizedCustomerPhone { get; set; }

        public void Init()
        {
            SerializedCreatedBy = CreatedBy.FullName;
            if (CustomerId.HasValue)
            {
                SerializedCustomerName = Customer.FullName;
                SeriazlizedCustomerPhone = Customer.Phone2 + (Customer.AdvertTypeId.HasValue ? (", " + Customer.AdvertType.Name) : "");
            }
        }

        public string TypeText
        {
            get
            {
                return IsIncoming ? "Входящий" : "Исходящий";
            }
        }

        public static string GetKindText(string Result, int ResultType, string Phone)
        {
            if (!String.IsNullOrWhiteSpace(Result)) return Result;

            if (ResultType == (int)CallResult.Cancelled)
            {
                return "Отмена";
            }
            else if (ResultType == (int)CallResult.NewCustomer)
            {
                return "Новый клиент (" + Phone + ")";
            }
            else if (ResultType == (int)CallResult.NotACustomer)
            {
                return "Не клиент";
            }
            else if (ResultType == (int)CallResult.OldCustomer)
            {
                return "Старый клиент";
            }
            else if (ResultType == (int)CallResult.Screnario)
            {
                return "Сценарий";
            }
            else if (ResultType == (int)CallResult.OK)
            {
                return "Успешно";
            }
            return "Неизвестно";
        }

        public string KindText
        {
            get
            {
                return GetKindText(Result, ResultType, SeriazlizedCustomerPhone);
            }
        }

        public static string GetComments(bool IsIncoming, string Log)
        {
            if (!IsIncoming)
            {
                return Log;
            }
            var x = Log.IndexOf("Комментарии:");
            if (x > -1)
            {
                var res = Log.Substring(x + 13);
                x = res.IndexOf("Сохранение нового");
                if (x > -1)
                {
                    return res.Substring(0, x - 2);
                }
                else
                {
                    x = res.IndexOf("Нажата кнопка");
                    if (x > -1)
                    {
                        return res.Substring(0, x - 2);
                    }
                }
                return res;
            }
            return String.Empty;
        }

        public string Comments
        {
            get
            {
                return GetComments(IsIncoming, Log);
            }
        }
    }
}
