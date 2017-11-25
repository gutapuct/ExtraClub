using System.Collections.Generic;
using TonusClub.Entities;
using System.Data;
using System.Linq;


namespace Sync.Models
{
    public static class Enumerators
    {
        public static Dictionary<int, string> Statuses;
        public static Dictionary<int, string> Categories;
        public static Dictionary<int, string> Priorities;

        static Enumerators()
        {
            Statuses = new Dictionary<int, string>();
            Statuses.Add(0, "Добавлена");
            Statuses.Add(1, "Отказ");
            Statuses.Add(2, "Взята в работу");
            Statuses.Add(3, "На проверке");
            Statuses.Add(4, "Закрыта");

            Categories = new Dictionary<int, string>();
            Categories.Add(0, "Бага");
            Categories.Add(1, "Доработка");
            Categories.Add(2, "Задача");
            Categories.Add(3, "Новая функциональность");

            Priorities = new Dictionary<int, string>();
            Priorities.Add(0, "Блокер");
            Priorities.Add(1, "Критически важно");
            Priorities.Add(2, "Важно");
            Priorities.Add(3, "Не важно");
            Priorities.Add(4, "Косметика");
        }
    }

    partial class Bug
    {
        public string Status
        {
            get
            {
                if (Enumerators.Statuses.ContainsKey(StatusId))
                    return Enumerators.Statuses[StatusId];
                return "Неизвестен";
            }
        }

        public string Category
        {
            get
            {
                if (Enumerators.Categories.ContainsKey(CategoryId))
                    return Enumerators.Categories[CategoryId];
                return "Неизвестна";
            }
        }

        public string Priority
        {
            get
            {
                if (Enumerators.Priorities.ContainsKey(PriorityId))
                    return Enumerators.Priorities[PriorityId];
                return "Неизвестен";
            }
        }
        public string CreatedName
        {
            get
            {
                using (var context = new TonusEntities())
                {
                    return context.Users.Single(u => u.UserId == CreatedBy).FullName;
                }
            }
        }

    }
}