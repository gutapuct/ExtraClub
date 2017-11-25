using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using TonusClub.Entities;

namespace TonusClub.ServerCore
{
    partial class CustomReports
    {
        public DataTable GetHoursLoad(DateTime start, DateTime end, Guid? divisionId)
        {
            end = end.Date.AddDays(1).AddMilliseconds(-1);
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var res = new DataTable();

                res.Columns.Add("Тренажер", typeof(string));

                if (!divisionId.HasValue)
                {
                    throw new FaultException<string>("Необходимо указать " + ClubText + "!", "Необходимо указать " + ClubText + "!");
                }

                var div = context.Divisions.First(i => i.Id == divisionId);
                var open = (div.OpenTime ?? new TimeSpan(7, 0, 0)).Hours;
                var close = (div.CloseTime ?? new TimeSpan(23, 0, 0)).Hours;
                for (int i = open; i < close; i++)
                {
                    res.Columns.Add(String.Format("{0}:00—{1}:00", i, i + 1), typeof(int));
                }

                var query = context.TreatmentEvents
                    .Where(i => i.VisitStatus != 1 && i.DivisionId == divisionId && i.VisitDate >= start && i.VisitDate < end)
                    .Select(i => new { i.TreatmentConfig.TreatmentType.Name, i.VisitDate.Hour })
                    .OrderBy(i=>i.Name)
                    .ToList();
                var row = new List<object> { "Все тренажеры" };
                for (int i = open; i < close; i++)
                {
                    row.Add(query.Count(j => j.Hour == i));
                }
                res.Rows.Add(row.ToArray());

                var q2 = query.GroupBy(i => i.Name);
                foreach (var t in q2)
                {
                    row = new List<object> { t.Key };
                    for (int i = open; i < close; i++)
                    {
                        row.Add(t.Count(j => j.Hour == i));
                    }
                    res.Rows.Add(row.ToArray());
                }

                return res;
            }
        }
    }
}
