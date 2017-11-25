using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace TonusClub.ServiceModel
{
    [DataContract]
    public class OrganizerItem
    {
        [DataMember]
        public object Data { get; set; }

        [DataMember]
        public string Text
        {
            get;
            set;
        }

        [DataMember]
        public string ClubText
        {
            get;
            set;
        }

        [DataMember]
        public DateTime AppearDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? ClosureDate
        {
            get;
            set;
        }

        [DataMember]
        public string Status
        {
            get;
            set;
        }

        [DataMember]
        public Guid? CustomerId { get; set; }

        [DataMember]
        public string Category
        {
            get;
            set;
        }

        [DataMember]
        public string SerializedCreatedBy
        {
            get;
            set;
        }

        [DataMember]
        public string SerializedAssignedTo
        {
            get;
            set;
        }

        [DataMember]
        public string CompletionComment
        {
            get;
            set;
        }

        [DataMember]
        public string SerializedClosedBy
        {
            get;
            set;
        }

        [DataMember]
        public DateTime ExpiryDate { get; set; }

        public string PriorityText
        {
            get
            {
                switch (Priority)
                {
                    case 0:
                        return "Высочайший";
                    case 1:
                        return "Высокий";
                    case 2:
                        return "Нормальный";
                    case 3:
                        return "Низкий";
                }
                return "(не указан)";
            }
        }

        public static string GetStatusText(int status)
        {

            switch (status)
            {
                case 0:
                    return "Поставлена";
                case 1:
                    return "Выполнена";
                case 2:
                    return "Отказ";
                case 3:
                    return "Отозвана";
            }
            return "(не указан)";

        }

        [DataMember]
        public int Priority { get; set; }

        public Brush Background
        {
            get
            {
                if (ExpiryDate < DateTime.Now) return Brushes.Salmon;
                if (ExpiryDate.Date == DateTime.Today) return Brushes.LightYellow;
                return Brushes.Transparent;
            }
        }

        //Гадкий телерик не умеет показывать грид с разными типами наследных классов, так что приходится уродоваться.

        public static OrganizerItem CreateCustomerTargetOrganizerItem(CustomerTarget target, Dictionary<Guid, string> clubs)
        {
            return new OrganizerItem
            {
                Data = target,
                AppearDate = target.CreatedOn,
                Category = String.Format("Тонусные дневники - цель ({0})", target.CustomerTargetType.Name),
                Status = target.Status,
                Text = target.Customer.FullName + ": " + target.TargetText,
                SerializedCreatedBy = "Автоматически",
                Priority = 2,
                ExpiryDate = target.TargetDate,
                SerializedAssignedTo = "Администраторы, инструкторы",
                ClosureDate = target.TargetDate,
                SerializedClosedBy = target.CreatedBy.FullName,
                CompletionComment = target.Comment,
                ClubText = target.Customer.ClubId.HasValue ? clubs[target.Customer.ClubId.Value] : "",
                CustomerId = target.CustomerId,
            };
        }

        public static OrganizerItem CreateDepositOutOrganizerItem(DepositOut dout)
        {
            dout.Init();
            return new OrganizerItem
            {
                Data = dout,
                AppearDate = dout.CreatedOn,
                Category = "Вывод средств с депозита",
                Status = GetStatusText(dout.ProcessedById.HasValue ? 1 : 0),
                Text = "Вывод средств по клиенту " + dout.Customer.FullName,
                SerializedCreatedBy = dout.CreatedBy.FullName,
                Priority = 2,
                ExpiryDate = dout.CreatedOn,
                SerializedAssignedTo = "Все сотрудники",
                ClosureDate = dout.ProcessedOn,
                SerializedClosedBy = dout.ProcessedById.HasValue ? dout.ProcessedBy.FullName : "",
                CompletionComment = dout.Comment,
                CustomerId = dout.CustomerId,
            };
        }

        public static OrganizerItem CreateCashlessPaymentOrganizerItem(BarOrder bo)
        {
            bo.Init();
            return new OrganizerItem
            {
                Data = bo,
                AppearDate = bo.PurchaseDate,
                Category = "Безналичная оплата",
                Status = GetStatusText(bo.PaymentDate.HasValue ? 1 : 0),
                Text = "Безналичная оплата по счету № " + bo.OrderNumber + " от " + bo.PurchaseDate.ToString("d"),
                SerializedCreatedBy = bo.CreatedBy.FullName,
                Priority = 3,
                ExpiryDate = bo.PurchaseDate.AddDays(30),
                SerializedAssignedTo = "Управляющий",
                ClosureDate = bo.PaymentDate,
                SerializedClosedBy = bo.CreatedBy.FullName,
                CompletionComment = bo.PaymentComments,
                CustomerId = bo.CustomerId,
            };
        }

        public static OrganizerItem CreateCustomerNotificationItem(CustomerNotification cn, Dictionary<Guid, string> clubs)
        {
            return new OrganizerItem
            {
                Data = cn,
                AppearDate = cn.CreatedOn,
                Category = "Оповещение клиента",
                Status = GetStatusText(cn.CompletionComment == "Отозвана" ? 3 : (cn.CompletedById.HasValue ? 1 : 0)),
                Text = GetSubjectWithDetails(cn),
                SerializedCreatedBy = cn.AuthorId.HasValue ? cn.CreatedBy.FullName : "Автоматически",
                Priority = cn.Priority,
                ExpiryDate = cn.ExpiryDate,
                SerializedAssignedTo = GetAttachString(cn.Employees),
                ClosureDate = cn.CompletedOn,
                SerializedClosedBy = cn.CompletedById.HasValue ? cn.CompletedBy.FullName : "",
                CompletionComment = "Задача:\n" + cn.Message + "\nОтчет:\n" + (String.IsNullOrEmpty(cn.CompletionComment) ? "Комментарий не указан" : cn.CompletionComment),
                ClubText = cn.Customer.ClubId.HasValue ? clubs[cn.Customer.ClubId.Value] : "",
                CustomerId = cn.CustomerId,
            };
        }

        private static string GetSubjectWithDetails(CustomerNotification p)
        {
            if (p.Subject == "Задача на обзвон клиентов")
            {
                return "Звонок " + p.Customer.FullName;
            }
            else
            {
                return p.Subject + " " + p.Customer.FullName;
            }
        }

        public static OrganizerItem CreateTaskOrganizerItem(Task t)
        {
            return new OrganizerItem
            {
                Data = t,
                AppearDate = t.CreatedOn,
                Category = "Прочее",
                Status = t.StatusText,
                Text = t.Subject,
                SerializedCreatedBy = t.AuthorId.HasValue ? t.CreatedBy.FullName : "Автоматически",
                Priority = t.Priority,
                ExpiryDate = t.ExpiryOn,
                SerializedAssignedTo = GetAttachString(t.Employees),
                ClosureDate = t.ClosedOn,
                SerializedClosedBy = t.ClosedById.HasValue ? t.ClosedBy.FullName : "",
                CompletionComment = "Задача:\n" + t.Message + "\nОтчет:\n" + (String.IsNullOrEmpty(t.ClosedComment) ? "Комментарий не указан" : t.ClosedComment),
            };
        }

        private static string GetAttachString(ICollection<Employee> Employees)
        {
            var sb = new StringBuilder();
            foreach (var i in Employees.OrderBy(i => i.BoundCustomer.FullName))
            {
                if (sb.Length != 0) sb.Append("; ");
                sb.AppendFormat("{0} {1}.{2}."
                    , i.BoundCustomer.LastName
                    , i.BoundCustomer.FirstName.Length > 0 ? i.BoundCustomer.FirstName[0] : ' '
                    , (i.BoundCustomer.MiddleName ?? "").Length > 0 ? i.BoundCustomer.MiddleName[0] : ' ');
            }
            if (sb.Length == 0) return "Все сотрудники";
            return sb.ToString();
        }
    }
}
