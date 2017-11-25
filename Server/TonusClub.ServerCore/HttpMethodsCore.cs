using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.Entities;
using System.Data;

namespace TonusClub.ServerCore
{
    public static class HttpMethodsCore
    {
        public static string GetZReport(Guid divId, DateTime dateS)
        {
            var res = new StringBuilder();
            using (var context = new TonusEntities())
            {
                dateS = dateS.Date;
                var dateF = dateS.AddDays(1);

                //Продажи из бара
                var bars = context.GoodSales
                    .Where(i => i.Storehouse.DivisionId == divId
                        && i.BarOrder.PurchaseDate >= dateS
                        && i.BarOrder.PurchaseDate < dateF
                        && i.PriceMoney.HasValue);
                foreach (var bs in bars)
                {
                    res.AppendFormat("{0};{1};{2};{3};{4};{5}\n", bs.Good.Code1C,
                        bs.Good.Name.Replace("\n", "_").Replace(";", "_"),
                        bs.Amount,
                        bs.PriceMoney.Value,
                        bs.Cost,
                        bs.GoodId
                    );
                }

                //Все остальное типизированное
                foreach (var bs in context.BarOrders
                    .Where(i => i.DivisionId == divId
                        && i.PurchaseDate >= dateS && i.PurchaseDate < dateF
                        && !i.ProviderId.HasValue
                        && (i.CardPayment > 0 || i.CashPayment > 0)
                        && i.Kind1C.HasValue)
                    .Join(context.Kinds1C, b => b.Kind1C, k => k.Id, (b, k) => new
                    {
                        GoodId = k.Code1C,
                        Name = k.Description,
                        Amount = 1,
                        Price = b.CashPayment > 0 ? b.CashPayment : b.CardPayment
                    }).ToArray())
                {
                    res.AppendFormat("{0};{1};{2};{3};{4};{5}\n",
                            bs.GoodId,
                            bs.Name,
                            bs.Amount,
                            bs.Price,
                            bs.Price,
                            bs.GoodId
                        );

                }
            }

            return res.ToString();
        }

        public static string GetWorkTime(Guid divisionId, DateTime start)
        {
            var res = new StringBuilder();
            var tbl = new CustomReports().GetEmployeesVisitReport(start, start.AddMonths(1), divisionId);
            foreach (DataRow row in tbl.Rows)
            {
                res.AppendFormat("{0};{1};{2:dd.MM.yyyy};{3};{4};{5};{6:n2}\n",
                row[1], row[3], row[2], row[6].ToString().Substring(0, 5), row[8].ToString().Substring(0, 5), row[10], row[11]
            );

            }
            return res.ToString();
        }

        public static string GetSalarySheet(Guid divisionId, DateTime start)
        {
            //1. Код сотрудника
            //2. ФИО
            //3. процент выполнения плана продаж по клубу
            //4. сумма к выплате
            //5. обоснование
            var res = new StringBuilder();
            using (var context = new TonusEntities())
            {
                var plan = SalaryCalculation.Get01TotalSalesPercent(context, divisionId, start, DateTime.MinValue);
                var ss = context.SalarySheets
                    .Include("SalarySheetRows")
                    .Include("SalarySheetRows.Employee")
                    .Include("SalarySheetRows.Employee.BoundCustomer")
                    .FirstOrDefault(i => i.PeriodStart == start);
                if (ss == null) return String.Empty;
                foreach (var ssr in ss.SalarySheetRows)
                {
                    res.AppendFormat("{0};{1};{2:n2};{3};{4}\n"
                        , ssr.Employee.Number
                        , ssr.Employee.BoundCustomer.FullName
                        , plan
                        , ssr.SalaryTotal
                        , ssr.Log.Replace(";", ".").Replace("\n", "|"));
                }
            }
            return res.ToString();
        }

        public static string GetPTU(Guid divisionId, DateTime date)
        {
            //0. Код поставщика
            //1/2. Поставщик
            //1. Код АСУ
            //2. Код 1С
            //3. Наименование
            //4. Количество
            //5. Цена за 1 ед.
            //6. Наименование единицы
            //7. Номер документа
            //8. Документ провел
            //9. ИНН поставщика
            var res = new StringBuilder();
            using (var context = new TonusEntities())
            {
                var d1 = date.AddDays(1);
                var items = context.ConsignmentLines
                    .Where(i => i.Consignment.DivisionId == divisionId && i.Consignment.Date >= date && i.Consignment.Date < d1 && i.Consignment.DocType == 0)
                    .Select(i => new
                    {
                        i.Consignment.ProviderId,
                        FullName = i.Consignment.Provider.FullName ?? i.Consignment.Provider.Name,
                        i.GoodId,
                        i.Good.Code1C,
                        i.Good.Name,
                        i.Quantity,
                        i.Price,
                        UnitTypeName = i.Good.UnitType.Name,
                        i.Consignment.Number,
                        i.Consignment.Prinal,
                        i.Consignment.Provider.INN
                    }).ToList();
                foreach (var item in items)
                {
                    res.AppendFormat("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}\n",
                        item.ProviderId, item.FullName, item.GoodId, item.Code1C, item.Name, item.Quantity, item.Price, item.UnitTypeName, item.Number, item.Prinal, item.INN);
                }
            }
            return res.ToString();
        }
    }
}
