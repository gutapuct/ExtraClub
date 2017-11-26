using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel.Employees
{
    public static class CoefficientTypes
    {
        public static Dictionary<int, string> TypeNames { get; private set; }
        static CoefficientTypes()
        {
            TypeNames = new Dictionary<int, string>();
            TypeNames.Add(1, "Выручка общая по всем позициям (% плана)");
            TypeNames.Add(2, "Выручка по группе продаж");
            TypeNames.Add(3, "Количество проданных абонементов");
            TypeNames.Add(4, "Количество абонементов, проданных со скидкой");
            TypeNames.Add(5, "Количество повторных проданных абонементов");
            TypeNames.Add(6, "Количество проданных абонементов определенного типа");
            TypeNames.Add(7, "Количество проданных карт определенного типа");
            TypeNames.Add(8, "Количество проданных товаров определенной группы");
            TypeNames.Add(9, "Количество проданного товара");
            TypeNames.Add(10, "Количество клиентов, принявших участие в акции бара");
            TypeNames.Add(11, "% клиентов, купивших абонемент после гостевого посещения");
            TypeNames.Add(12, "% клиентов, купивших абонемент после пробного посещения");
            TypeNames.Add(13, "Среднее количество ед, потраченных клиентом за день");
            TypeNames.Add(14, "Посещаемость клуба (% от макс. загрузки)");
            TypeNames.Add(15, "Посещаемость вида услуг (% от макс. загрузки)");
            TypeNames.Add(16, "Посещаемость услуги (тренажера) (% от макс. загрузки)");
            TypeNames.Add(17, "Посещаемость клуба по дням недели/часам (% от макс. загрузки)");
            TypeNames.Add(18, "Количество проданных сертификатов определенного типа");
            TypeNames.Add(19, "Привлеченных корп. клиентов");
            TypeNames.Add(20, "Выручка от продаж группе клиентов");
            TypeNames.Add(21, "Количество продаж группе клиентов");
            TypeNames.Add(22, "Количество абонементов, проданных клиентам с определенным видом карт");
            TypeNames.Add(23, "Выручка от абонементов, проданных клиентам с определенным видом карт");
            TypeNames.Add(24, "Выручка от продаж товаров клиентам с определенным видом карт");
            TypeNames.Add(25, "КПД");
            TypeNames.Add(26, "Процент от выручки (%)");
        }
    }
}
