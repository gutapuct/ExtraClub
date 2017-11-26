using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ExtraClub.Entities;

namespace ExtraClub.ServerCore
{
    partial class CustomReports
    {
        public DataTable ImcomingCalls(DateTime start, DateTime end, Guid? divisionId, bool days)
        {
            end = end.AddDays(1).AddSeconds(-1);
            var res = new DataTable();

            if (!divisionId.HasValue)
            {
                throw new Exception("Необходимо указать " + ClubTextR + "!");
            }
            using (var context = new ExtraEntities())
            {
                var div = context.Divisions.Single(i => i.Id == divisionId.Value);
                res.ExtendedProperties.Add("Detailed", true);
                res.Columns.Add("Критерий", typeof(string));
                AddDatesColumns(res, start, end, days, false, typeof(decimal), false);
                bool flag;

                var src = context.Calls.Where(i => i.DivisionId == divisionId && i.StartAt >= start && i.StartAt < end).Select(i => new { i.StartAt, i.ResultType }).ToArray();

                var row = CalcValue(new object[0], "Общее количество звонков", src, start, end, i => i.StartAt, days, false, out flag, false, (i, _, __) => i.Count());
                res.Rows.Add(row.ToArray());
                row = CalcValue(new object[0], "Успешно завершенных", src.Where(i => i.ResultType != 0), start, end, i => i.StartAt, days, false, out flag, false, (i, _, __) => i.Count());
                res.Rows.Add(row.ToArray());
                row = CalcValue(new object[0], "Отмененных", src.Where(i => i.ResultType == 0), start, end, i => i.StartAt, days, false, out flag, false, (i, _, __) => i.Count());
                res.Rows.Add(row.ToArray());
                row = CalcValue(new object[0], "Заведено новых клиентов", src.Where(i => i.ResultType == 3), start, end, i => i.StartAt, days, false, out flag, false, (i, _, __) => i.Count());
                res.Rows.Add(row.ToArray());
            }
            return res;
        }
    }
}
