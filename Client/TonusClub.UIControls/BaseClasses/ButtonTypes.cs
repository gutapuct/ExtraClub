using System.Collections.Generic;

namespace TonusClub.Infrastructure.BaseClasses
{
    public static class ButtonTypes
    {
        public static Dictionary<int, string> TypeNames { get; private set; }
        static ButtonTypes()
        {
            TypeNames = new Dictionary<int, string>();
            TypeNames.Add(0, "Сохранение истории звонка и завершение сценария");
            TypeNames.Add(1, "Открытие конкретной формы сценария");
            TypeNames.Add(2, "Открытие часовой сетки");
            TypeNames.Add(3, "Открытие сетки соляриев");
            TypeNames.Add(4, "Запись в солярий");
            TypeNames.Add(5, "Заведение нового клиента");
            TypeNames.Add(6, "Заведение нового клиента и запись его на услуги");
        }
    }
}
