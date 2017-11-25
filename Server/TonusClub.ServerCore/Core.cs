using System;
using System.Collections.Generic;
using System.Linq;

using TonusClub.Entities;
using TonusClub.ServiceModel;
using System.Reflection;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.ServiceModel;
using TonusClub.ServiceModel.Turnover;
using System.Xml.Serialization;
using System.IO;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using TonusClub.ServiceModel.Ssh;
using System.Data.Objects;
using System.Data.Objects.SqlClient;

namespace TonusClub.ServerCore
{
    public static partial class Core
    {
        public static string[] GetUserPermissions(Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                if(user == null) return new string[0];

                var res = user.Roles.SelectMany(i => i.Permissions).Select(perm => perm.PermissionKey).Distinct().ToList();

                //if (context.LocalSettings.Any()) res.Add("DisableCentral");
                //else
                {
                    try
                    {
                        using(var conn = new SqlConnection(((EntityConnection)context.Connection).StoreConnection.ConnectionString))
                        {
                            conn.Open();
                            var qr = new SqlCommand { Connection = conn, CommandText = "Select 1 from SyncMetadata.dbo.MetaCompanies where DivisionId=@c", CommandType = CommandType.Text }.AddParameter<Guid>("c", divisionId).ExecuteScalar();
                            conn.Close();
                            if(qr != null && qr != DBNull.Value)
                            {
                                res.Add("DisableRegional");
                            }
                        }
                    }
                    catch { }
                }

                return res.ToArray();
            }
        }

        public static bool DatesIntersects(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return end2 > start1 && end1 > start2;
        }

        public static bool DatesIntersectsEx(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return end2 >= start1 && end1 >= start2;
        }

        public static List<FoundCustomer> SearchCustomers(string searchKey)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                var res = new List<FoundCustomer>();
                if(String.IsNullOrEmpty(searchKey)) return res;
                searchKey = searchKey.Trim().ToLower();
                if(searchKey.StartsWith("+"))
                {
                    searchKey = searchKey.Substring(1);
                    var src = context.Customers
                        .Where(cl => cl.CompanyId == user.CompanyId)
                        .Where(cl => (cl.Phone1 ?? "").Contains(searchKey) || (cl.Phone2 ?? "").Contains(searchKey))
                        .Select(i => new { FullName = (i.LastName ?? "") + ((" " + i.FirstName) ?? "") + ((" " + i.MiddleName) ?? ""), Id = i.Id, ActiveCard = i.CustomerCards.Where(j => j.IsActive).OrderByDescending(j => j.EmitDate).FirstOrDefault() })
                        .ToList();
                    src.ForEach(c =>
                    {
                        //if (c.CustomerCards.Any(cc => cc.CardBarcode.Contains(searchKey) && cc.IsActive))
                        {
                            res.Add(new FoundCustomer { Id = c.Id, CardNumber = c.ActiveCard == null ? "" : c.ActiveCard.CardBarcode, FullName = c.FullName });
                        }
                    });

                }
                else
                {
                    var src = context.Customers
                        .Where(cl => cl.CompanyId == user.CompanyId)
                        .Where(cl => ((cl.LastName ?? "") + ((" " + cl.FirstName) ?? "") + ((" " + cl.MiddleName) ?? "")).Contains(searchKey) || cl.CustomerCards.Any(cc => cc.CardBarcode.Contains(searchKey) && cc.IsActive))
                       .Select(i => new { FullName = (i.LastName ?? "") + ((" " + i.FirstName) ?? "") + ((" " + i.MiddleName) ?? ""), Id = i.Id, ActiveCard = i.CustomerCards.Where(j => j.IsActive).OrderByDescending(j => j.EmitDate).Select(j => j.CardBarcode).FirstOrDefault() })
                       .Take(100)
                        .ToList();
                    src.ForEach(c =>
                    {
                        res.Add(new FoundCustomer { Id = c.Id, CardNumber = c.ActiveCard, FullName = c.FullName });
                    });
                }
                return res.ToList();
            }
        }

        public static Dictionary<string, DictionaryInfo> GetAllDictionaryInfos()
        {
            //if (_token == Guid.Empty) ThrowUnauthorizedAccessException();
            var res = new Dictionary<string, DictionaryInfo>();
            using(var context = new TonusEntities())
            {
                foreach(var di in context.DictionaryInfos)
                {
                    res.Add(di.EntitySetName, di);
                }
            }
            if(Thread.CurrentThread.CurrentCulture.Name != "ru-RU")
            {
                foreach(var key in res.Keys)
                {
                    res[key].DisplayName = res[key].DisplayNameEn;
                }
            }
            return res;
        }

        public static Dictionary<Guid, string> GetDictionaryList(string entitySetName)
        {
            var res = new BlockingCollection<KeyValuePair<Guid, string>>();
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var entitySet = String.Format("[{0}]", entitySetName);
                var query = context.CreateQuery<object>(entitySet);
                var dictInfo = context.DictionaryInfos.First(di => di.EntitySetName == entitySetName);
                var displayRowName = Thread.CurrentThread.CurrentCulture.Name == "ru-RU" ? dictInfo.DisplayRow : dictInfo.DisplayRowEn;
                PropertyInfo idProp = null, dispProp = null;
                try
                {
                    query.ToList().Where(i => (bool)i.GetValue(dictInfo.AvailRow)).ToList().ForEach(obj =>
                    {
                        var companyId = obj.GetValue("CompanyId");
                        if(companyId == null || companyId.Equals(user.CompanyId))
                        {
                            if(idProp == null) idProp = obj.GetType().GetProperty(dictInfo.IdRow);
                            if(dispProp == null) dispProp = obj.GetType().GetProperty(displayRowName);
                            res.Add(new KeyValuePair<Guid, string>((Guid)idProp.GetValue(obj, null), (string)dispProp.GetValue(obj, null)));
                        }
                    });
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            return res.ToDictionary(i => i.Key, i => i.Value);
        }

        /// <summary>
        /// Возвращает список идентификаторов созданных объектов
        /// </summary>
        /// <param name="entitySetName"></param>
        /// <param name="changes"></param>
        /// <returns></returns>
        public static IList<Guid> PostEntities(string entitySetName, IEnumerable changes)
        {
            using(var context = new TonusEntities())
            {
                var res = new List<Guid>();
                var user = UserManagement.GetUser(context);

                foreach(var obj in changes)
                {
                    object orig = null;
                    try
                    {
                        orig = context.GetObjectByKey(new EntityKey("TonusEntities." + entitySetName, "Id", obj.GetValue("Id")));
                    }
                    catch(ObjectNotFoundException) { }

                    if(orig == null && (bool)obj.GetValue("Deleted")) continue;

                    if((orig == null || obj.GetValue("Id").Equals(Guid.Empty)) && !(bool)obj.GetValue("Deleted"))
                    {
                        if(obj.GetValue("Id") == null || Guid.Empty.Equals(obj.GetValue("Id")))
                            obj.SetValue("Id", Guid.NewGuid());
                        obj.SetValue("AuthorId", user.UserId);
                        obj.SetValue("CreatedOn", DateTime.Now);
                        obj.SetValue("CompanyId", user.CompanyId);

                        context.AddObject(entitySetName, obj);
                    }
                    else
                    {
                        if((bool)obj.GetValue("Deleted"))
                        {
                            if(orig != null) context.ObjectStateManager.ChangeObjectState(orig, EntityState.Deleted);
                        }
                        else
                        {
                            context.Detach(orig);
                            context.AttachTo(entitySetName, obj);
                            context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                        }
                    }

                    res.Add((Guid)obj.GetValue("Id"));
                }

                context.SaveChanges();
                return res;
            }
        }

        public static List<T> GetAllRecords<T>(string entitySetName, bool companySnap = true)
        {
            using(var context = new TonusEntities())
            {
                var companyId = UserManagement.GetUser(context).CompanyId;
                var res = new List<T>();
                var entitySet = String.Format("[{0}]", entitySetName);
                var query = context.CreateQuery<T>(entitySet).ToArray();
                if(companySnap)
                {
                    res.AddRange(query.Where<T>(p => companyId.Equals(p.GetValue("CompanyId"))));
                }
                else
                {
                    res.AddRange(query);
                }
                if(typeof(T).GetInterface("IInitable") != null) res.ToList().ForEach(i => ((IInitable)i).Init());
                return res;
            }
        }

        public static List<GoodPrice> GetAllPrices(Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                return context.GoodPrices
                    .Where(i => i.DivisionId == divisionId)
                    .GroupBy(i => i.Good)
                    .Select(i => new { Good = i.Key, Price = i.OrderByDescending(j => j.Date).FirstOrDefault() })
                    .Where(i => i.Price.InPricelist)
                    .Select(i => new
                    {
                        AuthorId = i.Price.AuthorId,
                        BonusPrice = i.Price.BonusPrice,
                        Comments = i.Price.Comments,
                        CommonPrice = i.Price.CommonPrice,
                        CompanyId = i.Price.CompanyId,
                        Date = i.Price.Date,
                        DivisionId = i.Price.DivisionId,
                        EmployeePrice = i.Price.EmployeePrice,
                        GoodId = i.Good.Id,
                        Id = i.Price.Id,
                        RentFine = i.Price.RentFine,
                        RentPrice = i.Price.RentPrice,
                        SerializedCategory = i.Good.GoodsCategory.Name,
                        SerializedGoodName = i.Good.Name,
                        SerializedUnitType = i.Good.UnitType.Name
                    }).ToList().Select(i => new GoodPrice
                    {
                        AuthorId = i.AuthorId,
                        BonusPrice = i.BonusPrice,
                        Comments = i.Comments,
                        CommonPrice = i.CommonPrice,
                        CompanyId = i.CompanyId,
                        Date = i.Date,
                        DivisionId = i.DivisionId,
                        EmployeePrice = i.EmployeePrice,
                        GoodId = i.GoodId,
                        Id = i.Id,
                        InPricelist = true,
                        RentFine = i.RentFine,
                        RentPrice = i.RentPrice,
                        SerializedCategory = i.SerializedCategory,
                        SerializedGoodName = i.SerializedGoodName,
                        SerializedUnitType = i.SerializedUnitType
                    }).ToList();


                //var goods = new Dictionary<Guid, GoodPrice>();

                //foreach (var gp in context.GoodPrices
                //    .Where(gp => gp.DivisionId == divisionId).ToArray())
                //{
                //    if (!goods.ContainsKey(gp.GoodId))
                //    {
                //        goods.Add(gp.GoodId, gp);
                //    }
                //    else
                //    {
                //        if (goods[gp.GoodId].Date < gp.Date)
                //        {
                //            goods[gp.GoodId] = gp;
                //        }
                //    }
                //}
                //var res = goods.Values.Where(i => i.InPricelist).ToList();
                //res.ForEach(i => i.Init());
                //return res;
            }
        }

        public static List<BarPointGood> GetGoodsPresence(Guid divisionId)
        {
            var w = Stopwatch.StartNew();
            var res = new List<BarPointGood>();
            var dict = new Dictionary<Guid, List<BarPointGood>>();
            using(var context = new TonusEntities())
            {
                var division = context.Divisions.Include("Storehouses").Single(i => i.Id == divisionId);

                foreach(var store in division.Storehouses.Where(i => i.IsActive))
                {
                    context.Goods.Where(i => i.CompanyId == division.CompanyId)
                        .Select(i => new
                        {
                            Good = i,
                            Income = i.ConsignmentLines
                                .Where(l => l.Consignment.DestinationStorehouseId == store.Id && l.Consignment.IsAsset && (l.Consignment.DocType == 0 || l.Consignment.DocType == 1))
                                .Sum(l => (double?)l.Quantity) ?? 0,
                            Outcome1 = i.ConsignmentLines
                                .Where(l => l.Consignment.SourceStorehouseId == store.Id && l.Consignment.IsAsset && (l.Consignment.DocType == 1 || l.Consignment.DocType == 2))
                                .Sum(l => (double?)l.Quantity) ?? 0,
                            Outcome2 = i.GoodSales
                                .Where(s => s.StorehouseId == store.Id && !s.ReturnById.HasValue)
                                .Sum(l => (double?)l.Amount) ?? 0,
                            Outcome3 = i.Rents
                                .Count(r => (!r.FactReturnDate.HasValue || r.LostFine.HasValue) && r.StorehouseId == store.Id),
                            Outcome4 = i.CustomerGoodsFlows.Where(l => l.Amount > 0 && l.StorehouseId == store.Id)
                            .Sum(l => (int?)l.Amount) ?? 0
                        })
                        .Where(i => i.Income != i.Outcome1 + i.Outcome2 + i.Outcome3 + i.Outcome4)
                        .Select(i => new
                        {
                            GoodId = i.Good.Id,
                            Category = i.Good.GoodsCategory.Name,
                            Name = i.Good.Name,
                            Amount = i.Income - i.Outcome1 - i.Outcome2 - i.Outcome3 - i.Outcome4,
                            UnitName = i.Good.UnitType.Name
                        }).ToArray().Select(i => new BarPointGood
                        {
                            GoodId = i.GoodId,
                            Category = i.Category ?? "",
                            Name = i.Name,
                            Amount = i.Amount,
                            UnitName = i.UnitName ?? "ед.",
                            StorehouseId = store.Id,
                            StorehouseName = store.Name
                        }).ToList()
                        .ForEach(i =>
                        {
                            res.Add(i);
                        });
                }

                dict = res.GroupBy(i => i.GoodId).ToDictionary(i => i.Key, i => i.ToList());

                var prices = GetAllPrices(division.Id);
                prices.AsParallel().ForAll(price =>
                {
                    if(dict.ContainsKey(price.GoodId))
                    {
                        dict[price.GoodId].AsParallel().ForAll(i =>
                        {
                            i.Price = price.CommonPrice;
                            i.EmployeePrice = price.EmployeePrice ?? price.CommonPrice;
                            i.BonusPrice = (double?)price.BonusPrice;
                            i.RentPrice = price.RentPrice;
                            i.RentFine = price.RentFine;
                            i.IsInPricelist = true;
                        });
                    }

                });

                var st = context.Storehouses.FirstOrDefault(i => i.DivisionId == divisionId && i.BarSale && i.IsActive);
                if(st != null)
                {
                    context.Certificates.Where(i => i.DivisionId == divisionId && !i.BuyerId.HasValue).ToList().Init().ForEach(i =>
                    {
                        var bpg = new BarPointGood
                        {
                            GoodId = i.Id,
                            Name = String.IsNullOrWhiteSpace(i.Name) ? String.Format(Localization.Resources.CertFor, i.Amount) : i.Name,
                            Amount = 1,
                            UnitName = "шт.",
                            StorehouseId = st.Id,
                            StorehouseName = st.Name,
                            Price = i.PriceMoney ?? i.Amount,
                            EmployeePrice = i.PriceMoney ?? i.Amount,
                            BonusPrice = i.PriceBonus,
                            Category = i.SerializedCategoryName,
                            IsInPricelist = true
                        };
                        res.Add(bpg);
                    });
                    foreach(var p in context.Packages.Include("PackageLines").Where(i => i.CompanyId == st.CompanyId && i.IsActive).ToList())
                    {
                        if(p.PackageLines.All(l => res.Any(i => i.GoodId == l.GoodId && i.Amount >= l.Amount)))
                            res.Add(new BarPointGood
                            {
                                GoodId = p.Id,
                                Name = p.Name,
                                Amount = 1,
                                UnitName = "пакет",
                                StorehouseId = st.Id,
                                StorehouseName = st.Name,
                                Price = p.Price,
                                EmployeePrice = p.Price,
                                BonusPrice = null,
                                IsInPricelist = true
                            });
                    }
                }
                res = res.OrderBy(i => i.Name).ToList();
                Debug.WriteLine("GetGoodsPresence " + w.ElapsedMilliseconds + "ms");
                return res;
            }
        }

        public static List<Customer> GetPresentCustomers(Guid divisionId)
        {

            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var custs = from visit in context.CustomerVisits
                            join customer in context.Customers on visit.CustomerId equals customer.Id
                            where (visit.OutTime == null && visit.DivisionId == divisionId)
                            select customer;
                var lst = custs.ToList();
                var guest = context.Customers.FirstOrDefault(g => (g.LastName == "Гость" || g.LastName == "Guest") && g.CompanyId == user.CompanyId);
                if(guest != null) lst.Add(guest);
                lst.ForEach(c =>
                {
                    c.InitActiveCard();
                    c.InitDepositValues();
                });
                lst.Sort();
                return lst;
            }
        }

        public static string GetTicketNumber(TonusEntities context, Company company)
        {
            return company.ConcessionNumber + DateTime.Now.Year.ToString().Substring(2) + " " + (context.Tickets.Count(t => t.CompanyId == company.CompanyId && t.CreatedOn.Year == DateTime.Now.Year) + 1);
        }

        public static Customer GetCustomer(Guid customerId, bool loadDetails)
        {
            using(var context = new TonusEntities())
            {
                var companyId = UserManagement.GetCompanyIdOrDefaultId(context);
                ObjectQuery<Customer> src = context.Customers;
                if(loadDetails)
                {
                    src = src.Include("BonusAccounts")
                        .Include("DepositAccounts")
                        .Include("CustomerStatuses")
                        .Include("Corporate");
                }
                var res = src.FirstOrDefault(c => c.Id == customerId && c.CompanyId == companyId);
                if(res != null && loadDetails)
                {
                    InitCustomer(res, context);
                }
                return res;
            }
        }

        private static void InitCustomer(Customer res, TonusEntities context)
        {
            res.ActiveCard = context.CustomerCards.Include("CustomerCardType").Where(i => i.IsActive && i.CustomerId == res.Id).OrderByDescending(i => i.EmitDate).FirstOrDefault();
            if(res.ActiveCard != null)
            {
                res.ActiveCard.SerializedCustomerCardType = res.ActiveCard.CustomerCardType;
            }

            InitCustomerPresence(context, res);
            res.InitDepositValues();

            var shelves = context.CustomerShelves.Where(i => i.CustomerId == res.Id && !i.ReturnById.HasValue).Select(i => new { i.IsSafe, i.ShelfNumber }).ToList();
            res.ShelfNumber = shelves.Where(i => !i.IsSafe).Select(i => i.ShelfNumber).FirstOrDefault();
            if(res.ShelfNumber == 0) res.ShelfNumber = null;
            res.SafeNumber = shelves.Where(i => i.IsSafe).Select(i => i.ShelfNumber).FirstOrDefault();
            if(res.SafeNumber == 0) res.SafeNumber = null;

            res.CurrentBarDeposit = GetBarDiscountForCustomer(res.Id) / 100;

            res.SerializedCustomerCards = context.CustomerCards.Include("CustomerCardType").Where(i => i.CustomerId == res.Id).ToArray();
            foreach(var card in res.SerializedCustomerCards)
            {
                card.InitDetails();
            }

            if(res.InvitorId.HasValue)
            {
                InitCustomer(res.InvitedBy, context);
                res.InvitorText = res.InvitedBy.FullName;
                if(res.InvitedBy.ActiveCard != null) res.InvitorText += ", карта номер " + res.InvitedBy.ActiveCard.CardBarcode;
            }

            res.InitStatus();
        }

        private static void InitCustomerPresence(TonusEntities context, Customer res)
        {
            var t = context.CustomerVisits.Where(i => i.CustomerId == res.Id && !i.OutTime.HasValue).OrderByDescending(i => i.InTime).FirstOrDefault();
            if(t != null)
            {
                if(t.InTime.Date == DateTime.Today)
                {
                    res.PresenceStatusText = String.Format(ServiceModel.Localization.Resources.CustomerHereTime, t.Division.Name, t.InTime);
                }
                else
                {
                    res.PresenceStatusText = String.Format(ServiceModel.Localization.Resources.CustomerHereDate, t.Division.Name, t.InTime);
                }
                res.IsInClub = true;
            }


            if(String.IsNullOrEmpty(res.PresenceStatusText))
            {
                res.PresenceStatusText = ServiceModel.Localization.Resources.CustomerNotInClub;
            }
        }

        public static User GetCurrentUser()
        {
            using(var context = new TonusEntities())
            {
                return UserManagement.GetUser(context);
            }
        }

        public static int GetMaxPaymentNumber()
        {

            using(var context = new TonusEntities())
            {
                try
                {
                    var user = UserManagement.GetUser(context);
                    return context.BarOrders.Where(o => o.Division.CompanyId == user.CompanyId).Max(o => o.OrderNumber, 0);
                }
                catch { return 0; }
            }
        }

        public static Division GetDivision(Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var ls = context.LocalSettings.FirstOrDefault();
                if(ls != null && ls.DefaultDivisionId.HasValue)
                {
                    var div = context.Divisions.SingleOrDefault(i => i.Id == ls.DefaultDivisionId.Value);
                    if(div.CompanyId == user.CompanyId)
                    {
                        div.FirstCustomerDate = context.CustomerVisits.Where(i => i.DivisionId == div.Id).Min(i => (DateTime?)i.InTime);
                        return div;
                    }
                    else
                    {
                        throw new Exception("Пользователь не имеет доступа к клубу или неверный контекст клуба!");
                    }
                }


                var res = context.Divisions.FirstOrDefault(d => d.Id == divisionId);
                if(res == null || res.CompanyId != user.CompanyId)
                {
                    res = context.Divisions.FirstOrDefault(i => i.CompanyId == user.CompanyId);
                }
                if(res != null)
                {
                    res.FirstCustomerDate = context.CustomerVisits.Where(i => i.DivisionId == res.Id).Min(i => (DateTime?)i.InTime);
                }
                return res;

            }
        }

        public static IList<TicketType> GetTicketTypes(bool activeOnly)
        {
            using(var context = new TonusEntities())
            {
                List<TicketType> res;

                var user = UserManagement.GetUser(context);
                if(activeOnly)
                {
                    res = user.Company.TicketTypes.Where(i => i.IsActive).ToList();
                }
                else
                {
                    res = context.TicketTypes.Where(i => i.IsActive).ToList().Where(i => i.SettingsFolderId == null || user.Company.AvailSettingsFolders.Any(j => j.Id == i.SettingsFolderId)).ToList();
                    res.ForEach(i =>
                    {
                        if(user.Company.TicketTypes.Any(j => j.Id == i.Id)) i.Helper = true;
                    });
                }
                res.ForEach(i => i.Init());
                return res;
            }
        }

        public static List<TicketType> GetAllTicketTypes()
        {
            using(var context = new TonusEntities())
            {
                var res = context.TicketTypes.ToList();
                res.ForEach(i => i.Init());
                return res;
            }
        }

        public static List<CustomerCardType> GetAllCustomerCardTypes()
        {
            using(var context = new TonusEntities())
            {
                var res = context.CustomerCardTypes.ToList();
                return res;
            }
        }

        public static IEnumerable<TicketType> GetActiveTicketTypesForCustomer(Guid customerId)
        {

            using(var context = new TonusEntities())
            {
                var card = context.CustomerCards.Where(i => i.CustomerId == customerId && i.IsActive)
                    .OrderByDescending(i => i.EmitDate)
                    .Select(i => new { i.CompanyId, i.CustomerCardTypeId, i.CustomerCardType.IsVisit, i.CustomerCardType.IsGuest })
                    .FirstOrDefault();
                if(card == null) return new TicketType[0];
                var res = context.TicketTypes.Where(i => i.Companies.Any(j => j.CompanyId == card.CompanyId))
                    .Where(tt => tt.IsActive && tt.IsVisit == card.IsVisit && tt.IsGuest == card.IsGuest)
                    .Where(tt => !tt.CustomerCardTypes.Any() || tt.CustomerCardTypes.Any(j => j.Id == card.CustomerCardTypeId))
                    .OrderBy(i => i.Name).ToList();

                res.ForEach(i => i.Init());
                return res;
            }
        }

        public static IList<CustomerCardType> GetCustomerCardTypes(bool activeOnly)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                if(activeOnly)
                {
                    return user.Company.CustomerCardTypes.Where(i => i.IsActive).OrderBy(i => i.Name).ToList();
                }
                else
                {
                    var res = context.CustomerCardTypes
                        .Where(i => i.IsActive)
                        .OrderBy(i => i.Name)
                        .ToList()
                        .Where(i => i.SettingsFolderId == null || user.Company.AvailSettingsFolders.Any(j => j.Id == i.SettingsFolderId))
                        .ToList();
                    res.ForEach(i =>
                    {
                        if(user.Company.CustomerCardTypes.Any(j => j.Id == i.Id)) i.Helper = true;
                    });
                    return res;
                }
            }
        }

        public static bool RegisterCustomerVisit(Guid customerId, Guid divisionId, int shelfNumber, int safeNumber)
        {

            using(var context = new TonusEntities())
            {
                //if(context.Companies.Count() > 1 && divisionId != Guid.Parse("12A4128D-8258-4341-89C4-1F49B13F068D"))
                //{
                //    throw new FaultException<string>("На центральном сервере запрещена запись на услуги.", "На центральном сервере запрещена запись на услуги.");
                //}

                context.ContextOptions.LazyLoadingEnabled = false;
                var user = UserManagement.GetUser(context);
                var div = context.Divisions.Single(i => i.Id == divisionId);

                var customer = context.Customers.Single(i => i.Id == customerId);



                if(context.CustomerVisits.Any(i => i.CustomerId == customerId && !i.OutTime.HasValue))
                {
                    customer.InitCustomerPresence();
                    throw new FaultException<string>(customer.PresenceStatusText, customer.PresenceStatusText);
                }

                var cv = new CustomerVisit
                {
                    AuthorId = user.UserId,
                    Id = Guid.NewGuid(),
                    InTime = DateTime.Now,
                    CompanyId = div.CompanyId,
                    DivisionId = divisionId,
                    CustomerId = customerId
                };
                context.CustomerVisits.AddObject(cv);

                context.TreatmentEvents.Include("Ticket").Include("Ticket.TicektFreezes").Where(e => e.CustomerId == customerId &&
                    EntityFunctions.TruncateTime(e.VisitDate) == DateTime.Today &&
                    e.TicketId.HasValue &&
                    e.Ticket.TicketFreezes.Any(j => j.StartDate <= DateTime.Today && j.FinishDate > DateTime.Today)).SelectMany(i => i.Ticket.TicketFreezes)
                    .Where(f => f.StartDate <= DateTime.Today && f.FinishDate >= DateTime.Today).ToList().ForEach(i =>
                {
                    i.FinishDate = DateTime.Today.AddDays(-1);
                    if(i.FinishDate < i.StartDate) i.StartDate = i.FinishDate;
                });

                context.TreatmentEvents.Where(e => e.CustomerId == customerId
                    && EntityFunctions.TruncateTime(e.VisitDate) == DateTime.Today
                    && e.TicketId.HasValue && !e.Ticket.IsActive).Select(i => i.TicketId.Value).Distinct().ToList().ForEach(i =>
                {
                    ActivateTicket(i);
                });

                if(shelfNumber > -1)
                {
                    var sh = new CustomerShelf
                    {
                        AuthorId = user.UserId,
                        CompanyId = div.CompanyId,
                        CreatedOn = DateTime.Now,
                        CustomerId = customer.Id,
                        DivisionId = div.Id,
                        ShelfNumber = shelfNumber,
                        Id = Guid.NewGuid()
                    };
                    context.CustomerShelves.AddObject(sh);
                }

                if(safeNumber > -1)
                {
                    var sh = new CustomerShelf
                    {
                        AuthorId = user.UserId,
                        CompanyId = div.CompanyId,
                        CreatedOn = DateTime.Now,
                        CustomerId = customer.Id,
                        DivisionId = div.Id,
                        ShelfNumber = safeNumber,
                        Id = Guid.NewGuid(),
                        IsSafe = true
                    };
                    context.CustomerShelves.AddObject(sh);
                }

                context.SaveChanges();
                Logger.DBLog("Регистрация посещения клиента {0}, клуб {1}, шкафчик {2}, сейф {3}", customer.FullName, divisionId, shelfNumber, safeNumber);
                return true;
            }
        }

        public static Guid RegisterCustomerVisitEnd(Guid customerId, Guid divisionId, bool shelfReturned, bool safeReturned)
        {

            using(var context = new TonusEntities())
            {
                //if(context.Companies.Count() > 1 && divisionId != Guid.Parse("12A4128D-8258-4341-89C4-1F49B13F068D"))
                //{
                //    throw new FaultException<string>("На центральном сервере запрещена запись на услуги.", "На центральном сервере запрещена запись на услуги.");
                //}

                var user = UserManagement.GetUser(context);
                var vis = (from visit in context.CustomerVisits
                           where (!visit.OutTime.HasValue && visit.DivisionId == divisionId && visit.CustomerId == customerId)
                           select visit);
                var cnt = vis.Count();
                if(cnt == 0) throw new Exception(Localization.Resources.NoCustVisit);
                if(cnt > 1) throw new Exception("Exception:\nна пользователя зарегистрировано 2 визита в один клуб.");
                var division = context.Divisions.Single(j => j.Id == divisionId);

                var visi = vis.First();
                visi.OutTime = DateTime.Now;
                //visi.Receipt = receipt;
                foreach(var cr in context.ChildrenRooms.Where(i => !i.OutById.HasValue && i.CustomerId == customerId && i.DivisionId == divisionId))
                {
                    cr.OutTime = DateTime.Now;
                    cr.OutById = user.UserId;
                }

                foreach(var i in visi.Customer.CustomerShelves.Where(j => !j.ReturnById.HasValue && j.DivisionId == divisionId))
                {
                    i.ReturnOn = DateTime.Now;
                    i.ReturnById = user.UserId;
                    if(!shelfReturned && !i.IsSafe)
                    {
                        i.Penalty = i.Company.ShelfLostPenalty;
                        var tickets = visi.Customer.Tickets.ToList();
                        tickets.ForEach(k => k.InitDetails());
                        var ticket = tickets.FirstOrDefault(k => k.IsActive == true && k.UnitsLeft >= i.Company.ShelfLostPenalty);
                        if(ticket != null)
                        {
                            var tc = new UnitCharge
                            {
                                AuthorId = user.UserId,
                                Charge = 8, //i.Company.ShelfLostPenalty, //1 smart-тренировка (поле в настройках пока не нужно)
                                CompanyId = i.CompanyId,
                                Date = DateTime.Now,
                                Id = Guid.NewGuid(),
                                Reason = String.Format(Localization.Resources.CabinetFine, i.ShelfNumber),
                                TicketId = ticket.Id
                            };
                            context.UnitCharges.AddObject(tc);
                            //division.ShelvesRepository = division.ShelvesRepository.Replace(i.ShelfNumber + ",", "");
                            if (!division.ShelvesRepository.EndsWith(","))
                            {
                                division.ShelvesRepository = division.ShelvesRepository + ",";
                            }
                            division.ShelvesRepository = division.ShelvesRepository.Remove(division.ShelvesRepository.IndexOf(i.ShelfNumber.ToString()), i.ShelfNumber.ToString().Count() + 1);
                        }
                    }
                    if(!safeReturned && i.IsSafe)
                    {
                        i.Penalty = i.Company.SafeLostPenalty;
                        var tickets = visi.Customer.Tickets.ToList();
                        tickets.ForEach(k => k.InitDetails());
                        var ticket = tickets.FirstOrDefault(k => k.IsActive == true && k.UnitsLeft >= i.Company.SafeLostPenalty);
                        if(ticket != null)
                        {
                            var tc = new UnitCharge
                            {
                                AuthorId = user.UserId,
                                Charge = i.Company.SafeLostPenalty,
                                CompanyId = i.CompanyId,
                                Date = DateTime.Now,
                                Id = Guid.NewGuid(),
                                Reason = String.Format(Localization.Resources.SafeFine, i.ShelfNumber),
                                TicketId = ticket.Id
                            };
                            context.UnitCharges.AddObject(tc);
                            division.SafesRepository = division.SafesRepository.Replace(i.ShelfNumber + ",", "");
                        }
                    }
                }

                if(IsFirstVisitEnabled(customerId))
                {
                    ProcessAcquaintanceCorrection(context, visi.Customer);
                }


                context.SaveChanges();
                Logger.DBLog("Регистрация выхода клиента из клуба. Клиент {0}, клуб {1}", visi.Customer.FullName, division.Name);
                return visi.Id;
            }
        }

        private static void ProcessAcquaintanceCorrection(TonusEntities context, Customer customer)
        {
            var date = DateTime.Today.AddDays(1);
            var events = context.TreatmentEvents.Where(i => i.CustomerId == customer.Id && i.VisitDate >= DateTime.Today && i.VisitDate < date && (i.VisitStatus == 2 || i.VisitStatus == 3)).ToList();
            var matched = new Dictionary<Guid, UnitCharge>();
            foreach(var ev in events)
            {
                if(matched.ContainsKey(ev.TreatmentConfigId)) continue;
                var charge = context.UnitCharges.FirstOrDefault(i => i.EventId == ev.Id);
                if(charge != null)
                {
                    matched.Add(ev.TreatmentConfigId, charge);
                }
            }
            if(matched.Count > 0)
            {
                bool flag = true;
                foreach(var pair in matched)
                {
                    if(flag)
                    {
                        pair.Value.Charge = 1;
                        pair.Value.Reason = String.Format(Localization.Resources.TestVisit, matched.Count);
                        flag = false;
                    }
                    else
                    {
                        context.DeleteObject(pair.Value);
                    }
                }
            }
        }

        public static void PostGoodAction(Guid actionId, string actionName, double discount, IEnumerable<KeyValuePair<Guid, int>> goods, bool isActive)
        {

            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                GoodAction action;
                if(actionId == Guid.Empty)
                {
                    action = new GoodAction
                    {
                        AuthorId = user.UserId,
                        CompanyId = user.CompanyId,
                        CreatedOn = DateTime.Now,
                        Id = Guid.NewGuid()
                    };
                }
                else
                {
                    action = context.GetObjectByKey(new EntityKey("TonusEntities.GoodActions", "Id", actionId)) as GoodAction;
                    foreach(var al in action.GoodActionLines.ToArray())
                    {
                        context.DeleteObject(al);
                    }
                    action.GoodActionLines.Clear();
                }

                action.Discount = (decimal)discount;
                action.IsActive = isActive;
                action.Name = actionName;



                foreach(var g in goods)
                {
                    action.GoodActionLines.Add(new GoodActionLine
                    {
                        GoodActionId = action.Id,
                        GoodId = g.Key,
                        Id = Guid.NewGuid(),
                        Amount = g.Value,
                        CompanyId = user.CompanyId

                    });
                }
                if(actionId == Guid.Empty)
                {
                    context.GoodActions.AddObject(action);
                }
                context.SaveChanges();
                Logger.DBLog("Сохранение/добавление акции на товар, название {1}", actionId, actionName);
            }
        }

        public static void DeleteGoodAction(Guid goodActionId)
        {

            using(var context = new TonusEntities())
            {
                var ga = context.GoodActions.FirstOrDefault(a => a.Id == goodActionId);
                if(ga != null)
                {

                    ga.GoodActionLines.ToList().ForEach(al => context.DeleteObject(al));
                    context.DeleteObject(ga);
                    context.SaveChanges();
                    Logger.DBLog("Удаление акции на товар {0}", ga.Name);
                }
            }
        }

        public static void SetObjectActive(string collectionName, Guid id, bool isActive)
        {

            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                var obj = context.GetObjectByKey(new EntityKey("TonusEntities." + collectionName, "Id", id));
                if(obj != null)
                {
                    obj.SetValue("IsActive", isActive);
                    context.SaveChanges();
                }
            }
        }

        public static void PostGoodPrice(GoodPrice goodPrice)
        {

            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var price = new GoodPrice
                {
                    Id = Guid.NewGuid(),
                    AuthorId = user.UserId,
                    CompanyId = user.Company.CompanyId,
                    DivisionId = goodPrice.DivisionId,
                    GoodId = goodPrice.GoodId,
                    CommonPrice = goodPrice.CommonPrice,
                    EmployeePrice = goodPrice.EmployeePrice,
                    BonusPrice = goodPrice.BonusPrice,
                    InPricelist = goodPrice.InPricelist,
                    Date = DateTime.Now,
                    Comments = goodPrice.Comments,
                    RentFine = goodPrice.RentFine,
                    RentPrice = goodPrice.RentPrice
                };

                var good = context.Goods.Where(i => i.Id == goodPrice.GoodId).Select(i => i.Name).FirstOrDefault();
                var div = context.Divisions.Where(i => i.Id == goodPrice.DivisionId).Select(i => i.Name).FirstOrDefault();

                context.GoodPrices.AddObject(price);
                context.SaveChanges();
                Logger.DBLog("Задание цены на товар {0}, клуб {1}, цена {2}", good, div, goodPrice.CommonPrice);
            }
        }

        public static void PostProviderPayment(Guid orderId, DateTime date, string paymentType, decimal amount, string comment)
        {

            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var order = context.Consignments.Single(i => i.Id == orderId);

                var pp = new ProviderPayment
                {
                    Amount = amount,
                    AuthorId = user.UserId,
                    Comment = comment,
                    Id = Guid.NewGuid(),
                    Number = context.ProviderPayments.Where(i => i.Order.DivisionId == order.DivisionId).Max(i => i.Number ?? 0, 0) + 1,
                    OrderId = orderId,
                    ProviderId = order.ProviderId.Value,
                    Date = date,
                    PaymentType = paymentType,
                    CompanyId = order.CompanyId
                };
                context.ProviderPayments.AddObject(pp);

                var stId = context.SpendingTypes.Single(i => i.Name == Localization.Resources.GoodSpendings && i.CompanyId == user.CompanyId).Id;

                var sp = new Spending
                {
                    Amount = amount,
                    AuthorId = user.UserId,
                    CompanyId = order.CompanyId,
                    CreatedOn = DateTime.Now,
                    DivisionId = order.DivisionId.Value,
                    Id = Guid.NewGuid(),
                    Name = String.Format(Localization.Resources.ProviderPaymentNum, order.Provider.Name, order.Number),
                    Number = context.Spendings.Where(i => i.DivisionId == order.DivisionId.Value).Max(i => i.Number, 0) + 1,
                    PaymentType = Localization.Resources.ProviderPayment,
                    SpendingTypeId = stId
                };
                context.Spendings.AddObject(sp);
                context.SaveChanges();
            }
        }

        public static void ActivateTicket(Guid ticketId)
        {

            using(var context = new TonusEntities())
            {
                var ticket = context.Tickets.FirstOrDefault(t => t.Id == ticketId);
                ticket.InitDetails();

                if(ticket.Status == TicketStatus.Available)
                {
                    //Разрешаем ручную активацию. допускаем несколько активных абонементов.
                    //if (context.Tickets.Any(t => t.IsActive && t.CustomerId == ticket.CustomerId && t.DivisionId == ticket.DivisionId))
                    //{
                    //    return;
                    //}

                    ticket.IsActive = true;
                    if(!ticket.StartDate.HasValue) ticket.StartDate = DateTime.Today;
                    context.SaveChanges();
                    Logger.DBLog("Активация абонемента {0}", ticket.Number);
                }
            }
        }

        public static Guid PostNewDictionaryElement(Guid dictionaryId, string newElementName)
        {

            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var dictInfo = context.DictionaryInfos.FirstOrDefault(di => di.Id == dictionaryId);
#if BEAUTINIKA
                var type = Type.GetType(String.Format("TonusClub.ServiceModel.{0}, Beautinika.ServiceModel", dictInfo.EntityTypeName));
#else
                var type = Type.GetType(String.Format("TonusClub.ServiceModel.{0}, TonusClub.ServiceModel", dictInfo.EntityTypeName));
#endif

                TestExistance(context, type, dictInfo, newElementName, StringComparison.InvariantCultureIgnoreCase);

                var obj = type.GetConstructor(new Type[0]).Invoke(new object[0]);

                var newGuid = Guid.NewGuid();

                obj.SetValue(dictInfo.IdRow, newGuid);
                obj.SetValue(dictInfo.DisplayRow, newElementName.Trim());
                obj.SetValue(dictInfo.DisplayRowEn, newElementName.Trim());
                obj.SetValue("CreatedOn", DateTime.Now);
                obj.SetValue("AuthorId", user.UserId);
                obj.SetValue("CompanyId", user.CompanyId);
                obj.SetValue(dictInfo.AvailRow, true);


                context.AddObject(dictInfo.EntitySetName, obj);
                context.SaveChanges();
                return newGuid;
            }
        }

        private static void TestExistance(TonusEntities context, Type elementType, DictionaryInfo dictInfo, string elementName, StringComparison comparison)
        {
            var displayRowName = Thread.CurrentThread.CurrentCulture.Name == "ru-RU" ? dictInfo.DisplayRow : dictInfo.DisplayRowEn;
            var mi = context.GetType().GetMethod("CreateQuery").MakeGenericMethod(elementType);
            var query = mi.Invoke(context, new object[] { dictInfo.EntitySetName, new System.Data.Objects.ObjectParameter[0] });
            if(query.ExecuteToArrayMethod().Any(o => String.Equals(elementName, (o.GetValue(displayRowName) ?? "").ToString(), comparison) && (bool)o.GetValue(dictInfo.AvailRow)))
            {
                throw new DuplicateNameException(Localization.Resources.ElementExists);
            }

        }

        public static void PostRenameDictionaryElement(Guid dictionaryId, Guid elementGuid, string elementName)
        {

            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var dictInfo = context.DictionaryInfos.FirstOrDefault(di => di.Id == dictionaryId);
                var type = Type.GetType(String.Format("TonusClub.ServiceModel.{0}, TonusClub.ServiceModel", dictInfo.EntityTypeName));

                TestExistance(context, type, dictInfo, elementName, StringComparison.InvariantCulture);

                var obj = context.GetObjectByKey(new System.Data.EntityKey("TonusEntities." + dictInfo.EntitySetName, dictInfo.IdRow, elementGuid));

                if(obj == null) throw new NullReferenceException(Localization.Resources.NoElement);
                if(Thread.CurrentThread.CurrentCulture.Name == "ru-RU")
                {
                    obj.SetValue(dictInfo.DisplayRow, elementName.Trim());
                }
                else
                {
                    obj.SetValue(dictInfo.DisplayRowEn, elementName.Trim());
                }
                obj.SetValue("ModifiedOn", DateTime.Now);
                obj.SetValue("AuthorId", user.UserId);

                context.SaveChanges();
            }
        }

        public static string PostRemoveDictionaryElement(Guid dictionaryId, Guid elementGuid)
        {

            using(var context = new TonusEntities())
            {
                var dictInfo = context.DictionaryInfos.FirstOrDefault(di => di.Id == dictionaryId);
                var obj = context.GetObjectByKey(new System.Data.EntityKey("TonusEntities." + dictInfo.EntitySetName, dictInfo.IdRow, elementGuid));

                if(obj == null) return Localization.Resources.NoElement;

                try
                {
                    obj.SetValue(dictInfo.AvailRow, false);
                    //context.DeleteObject(obj);
                    context.SaveChanges();
                }
                catch(UpdateException e)
                {
                    if(e.InnerException is System.Data.SqlClient.SqlException)
                    {
                        return Localization.Resources.ElementRefs;
                    }
                }
                return String.Empty;
            }
        }

        public static T GetOneRecord<T>(string setName, string keyName, Guid keyValue, bool init = false)
        {

            using(var context = new TonusEntities())
            {
                var res = (T)context.GetObjectByKey(new EntityKey(setName, keyName, keyValue));
                if(init)
                {
                    ((IInitable)res).Init();
                }
                return res;
            }
        }

        public static Customer GetCustomerById(int cardNumber, bool loadDetails)
        {

            using(var context = new TonusEntities())
            {
                try
                {
                    var companyId = UserManagement.GetCompanyIdOrDefaultId(context);
                    var sn = cardNumber.ToString();
                    var customerId = context.CustomerCards.Where(c => c.CardBarcode == sn && c.IsActive && c.CompanyId == companyId).Select(i => i.CustomerId).FirstOrDefault();
                    if(customerId == Guid.Empty) return null;
                    ObjectQuery<Customer> src = context.Customers;
                    if(loadDetails)
                    {
                        src = src.Include("BonusAccounts")
                            .Include("DepositAccounts")
                            .Include("CustomerStatuses")
                            .Include("Corporate");
                    }
                    var res = src.FirstOrDefault(c => c.Id == customerId);
                    if(res != null && loadDetails)
                    {
                        InitCustomer(res, context);
                    }

                    return res;
                }
                catch(ObjectNotFoundException)
                {
                    return null;
                }
            }
        }

        public static void UpdateCustomerForm(Customer customer)
        {

            using(var context = new TonusEntities())
            {
                var originalCustomer = context.GetObjectByKey(new EntityKey("TonusEntities.Customers", "Id", customer.Id)) as Customer;
                if(originalCustomer != null)
                {
                    originalCustomer.FirstName = customer.FirstName;
                    originalCustomer.MiddleName = customer.MiddleName;
                    originalCustomer.LastName = customer.LastName;
                    originalCustomer.Birthday = customer.Birthday;

                    originalCustomer.PasspEmitDate = customer.PasspEmitDate;
                    originalCustomer.PasspEmitPlace = customer.PasspEmitPlace;
                    originalCustomer.PasspNumber = customer.PasspNumber;

                    originalCustomer.Phone1 = customer.Phone1;
                    originalCustomer.Phone2 = customer.Phone2;
                    originalCustomer.Email = customer.Email;
                    originalCustomer.SmsList = customer.SmsList;

                    originalCustomer.AddrIndex = customer.AddrIndex;
                    originalCustomer.AddrCity = customer.AddrCity;
                    originalCustomer.AddrStreet = customer.AddrStreet;
                    originalCustomer.AddrOther = customer.AddrOther;
                    originalCustomer.AddrMetro = customer.AddrMetro;

                    originalCustomer.ManagerId = customer.ManagerId;
                    originalCustomer.AdvertTypeId = customer.AdvertTypeId;
                    originalCustomer.AdvertComment = customer.AdvertComment;

                    originalCustomer.Kids = customer.Kids;
                    originalCustomer.SocialStatusId = customer.SocialStatusId;
                    originalCustomer.WorkPlace = customer.WorkPlace;
                    originalCustomer.WorkPhone = customer.WorkPhone;
                    originalCustomer.Position = customer.Position;

                    originalCustomer.HasEmail = customer.HasEmail;
                    originalCustomer.IsWork = customer.IsWork;

                    originalCustomer.Comments = customer.Comments;

#if BEAUTINIKA
                    originalCustomer.MaterialCoeff = customer.MaterialCoeff;
#endif

                    context.SaveChanges();
                }
            }
        }

        public static List<string>[] GetAddressLists()
        {
            using(var context = new TonusEntities())
            {
                var res = new List<string>[3];
                res[0] = (from c in context.Customers
                          select c.AddrCity).Distinct().ToList();
                res[1] = (from c in context.Customers
                          select c.AddrMetro).Distinct().ToList();
                res[2] = (from c in context.Customers
                          select c.AddrStreet).Distinct().ToList();
                res[2].AddRange(context.Employees.Select(i => i.FactStreet));
                res[2] = res[2].Distinct().ToList();
                return res;
            }
        }

        public static List<decimal> GetDiscountsForCurrentUser(short discountType)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                IEnumerable<string> discounts = new string[0];
                if(discountType == 1) discounts = user.Roles.Select(i => i.CardDiscs);
                if(discountType == 2) discounts = user.Roles.Select(i => i.TicketDiscs);
                var res = new List<decimal>();
                foreach(var disc in discounts)
                {
                    foreach(var d in (disc ?? "").Replace(" ", "").Split(',').Select(i => i.Replace(".", ",")))
                    {
                        decimal di;
                        if(Decimal.TryParse(d, out di) && !res.Contains(di / 100)) res.Add(di / 100);
                    }
                }
                if(discountType == 2)
                {
                    foreach(var disc in user.Roles.Select(i => i.TicketRubDiscs))
                    {
                        foreach(var d in (disc ?? "").Replace(" ", "").Split(',').Select(i => i.Replace(".", ",")))
                        {
                            decimal di;
                            if(Decimal.TryParse(d, out di) && !res.Contains(-di)) res.Add(-di);
                        }
                    }
                }

                return res;
            }
        }

        public static string UpdateInvitor(Guid invitedId, Guid invitorId)
        {
            using(var context = new TonusEntities())
            {
                var invitor = context.GetObjectByKey(new EntityKey("TonusEntities.Customers", "Id", invitorId)) as Customer;
                if(!invitor.Tickets.Any(t => t.IsActive)) return Localization.Resources.InvNoTickets;
                invitor.Tickets.Where(i => i.IsActive).ToList().ForEach(i => i.InitDetails());
                if(invitor.Tickets.Where(t => t.IsActive).Sum(t => t.GuestUnitsLeft) == 0) return Localization.Resources.InvNoUnits;
                if(!invitor.Tickets.Any(t => t.Customer.CustomerVisits.Any(v => !v.OutTime.HasValue))) return Localization.Resources.InvOut;

                var customer = context.GetObjectByKey(new EntityKey("TonusEntities.Customers", "Id", invitedId)) as Customer;
                customer.InvitorId = invitorId;
                context.SaveChanges();

                Logger.DBLog("Обновление информации о пригласившем клиенте. Кто: {0}, кого: {1}", invitor.FullName, customer.FullName);

                return String.Empty;
            }
        }


        public static int GetMaxGuestUnits(Guid divisionId, Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                var maxGuestUnits = context.Divisions.Where(i => i.Id == divisionId).Select(i => i.Company.MaxGuestUnits).Single();

                var units = (context.Tickets.Where(i => i.IsActive && i.CustomerId == customerId).Sum(i => (int?)i.GuestUnitsAmount) ?? 0)
                    - (context.UnitCharges.Where(i => i.Ticket.IsActive && i.Ticket.CustomerId == customerId).Sum(i => (int?)i.GuestCharge) ?? 0);

                //var tickets = customer.Tickets.Where(t => t.IsActive && t.DivisionId == divisionId).ToList();
                //tickets.ForEach(t => t.InitDetails());
                //var ticket = tickets.Where(t => t.GuestUnitsLeft > 0).OrderBy(t => t.FinishDate).FirstOrDefault();
                //if (ticket != null)
                //{
                //    return (int)ticket.GuestUnitsLeft;
                //}
                return (int)Math.Min(units, maxGuestUnits);
            }
        }

        public static void PostCustomerAddress(Guid customerId, string metro, string index, string city, string street, string other)
        {
            using(var context = new TonusEntities())
            {
                var customer = context.GetObjectByKey(new EntityKey("TonusEntities.Customers", "Id", customerId)) as Customer;
                customer.AddrCity = city;
                customer.AddrIndex = index;
                customer.AddrMetro = metro;
                customer.AddrOther = other;
                customer.AddrStreet = street;


                context.SaveChanges();
                Logger.DBLog("Обновление адреса клиента. {0}", customer.FullName);
            }
        }

        public static void DeleteObject(string collectionName, Guid id)
        {
            using(var context = new TonusEntities())
            {
                try
                {
                    var obj = context.GetObjectByKey(new EntityKey("TonusEntities." + collectionName, "Id", id));
                    context.DeleteObject(obj);
                    context.SaveChanges();
                }
                catch(UpdateException ex)
                {
                    throw new FaultException<string>(ex.Message, new FaultReason(Localization.Resources.DelRefs));
                }
            }
        }

        public static void PostTicketTypeTreatmentTypes(Guid ticketTypeId, List<Guid> treatmentTypes)
        {
            using(var context = new TonusEntities())
            {
                var tt = context.GetObjectByKey(new EntityKey("TonusEntities.TicketTypes", "Id", ticketTypeId)) as TicketType;
                tt.TreatmentTypes.Clear();
                foreach(var i in treatmentTypes)
                {
                    tt.TreatmentTypes.Add(context.GetObjectByKey(new EntityKey("TonusEntities.TreatmentTypes", "Id", i)) as TreatmentType);
                }
                context.SaveChanges();
            }
        }

        public static void StartTicketReturn(Guid ticketId, string comment)
        {
            //TODO:
            //заморозка бессрочно, постановка задачи.
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var ticket = context.Tickets.SingleOrDefault(t => t.Id == ticketId);
                if(ticket == null) return;
                var freeze = new TicketFreeze
                {
                    AuthorId = user.UserId,
                    CompanyId = ticket.CompanyId,
                    CreatedOn = DateTime.Now,
                    FinishDate = DateTime.Today.AddDays(3654),
                    StartDate = DateTime.Today,
                    TicketFreezeReasonId = Guid.Empty,
                    TicketId = ticketId,
                    Comment = comment,
                    Id = Guid.NewGuid()
                };
                context.TicketFreezes.AddObject(freeze);


                context.SaveChanges();
                Logger.DBLog("Инициирование возврата абонемента. {0} ({1})", ticket.Number, comment);
            }
        }

        public static List<TreatmentEvent> GetCustomerEvents(Guid customerId, DateTime start, DateTime end, bool canceled)
        {
            using(var context = new TonusEntities())
            {
                var startX = start.AddHours(-2);
                var res = context.TreatmentEvents
                    .Where(i => i.CustomerId == customerId && i.VisitDate >= startX && i.VisitDate <= end && (canceled || i.VisitStatus != 1))
                    .Select(i => new
                    {
                        Id = i.Id,
                        VisitDate = i.VisitDate,
                        EndTime = EntityFunctions.AddMinutes(i.VisitDate, i.TreatmentConfig.LengthCoeff * i.TreatmentConfig.TreatmentType.Duration) ?? i.VisitDate,
                        SerializedTicketNumber = i.Ticket.Number,
                        SerializedTreatmentTypeName = i.TreatmentConfig.TreatmentType.Name,
                        SerializedTreatmentName = i.Treatment.Tag ?? i.Treatment.TreatmentType.Name,
                        VisitStatus = i.VisitStatus,
                        Price = i.TreatmentConfig.Price,
                        Cost = (int?)context.UnitCharges.Where(x => x.EventId == i.Id).FirstOrDefault().Charge,
#if BEAUTINIKA
                        CostExtra = (int?)context.UnitCharges.Where(x => x.EventId == i.Id).FirstOrDefault().ExtraCharge,
#endif
                        Comment = i.Comment,
                        TicketId = i.TicketId,
                        Employer = i.ModifiedBy.HasValue ? i.ModifiedUser.FullName : i.CreatedBy.FullName,
                        SerializedTreatmentTypeId = i.TreatmentConfig.TreatmentTypeId,
                        Duration = i.TreatmentConfig.LengthCoeff * i.TreatmentConfig.TreatmentType.Duration,
                        i.TreatmentConfigId,
#if BEAUTINIKA
                        IsMain = i.TreatmentConfig.IsMainTreatment,
                        RoomName = i.Room.Name,
                        EmployeeName = i.Employee.BoundCustomer.LastName,
                        HasGoodCharges = i.GoodCharges.Any()
#endif
                    })
                    .OrderByDescending(i => i.VisitDate)
                    .ToList()
                    .Where(i => i.VisitDate.AddMinutes(i.Duration) >= start).ToList();

                var res1 = res.Select(i => new TreatmentEvent
                    {
                        Id = i.Id,
                        VisitDate = i.VisitDate,
                        EndTime = i.EndTime,
                        SerializedTicketNumber = i.SerializedTicketNumber,
                        SerializedTreatmentTypeName = i.SerializedTreatmentTypeName,
                        SerializedTreatmentName = i.SerializedTreatmentName,
                        VisitStatus = i.VisitStatus,
                        Price = i.Price,
                        TreatmentConfigId = i.TreatmentConfigId,
                        Cost = i.Cost ?? 0,
                        Employer = i.Employer,
#if BEAUTINIKA
                        CostExtra = i.CostExtra ?? 0,
#endif
                        Comment = i.Comment,
                        TicketId = i.TicketId,
                        SerializedTreatmentTypeId = i.SerializedTreatmentTypeId,
                        SerializedDuration = i.Duration,
#if BEAUTINIKA
                        IsMainTreatment = i.IsMain,
                        SerializedRoomName = i.RoomName,
                        SerializedEmployeeName = i.EmployeeName,
                        HasGoodCharges = i.HasGoodCharges ? "Да" : ""
#endif
                    })
                    .ToList();
#if !BEAUTINIKA
                foreach(var item in context.UnitCharges.Where(
                    i =>
                        i.Ticket.CustomerId == customerId && i.Date >= startX && i.Date <= end &&
                        i.Charge > 0 &&
                        i.Reason == "Автоматическое списание для смарт-абонементов")
                    .Select(i => new
                    {
                        i.Date,
                        i.Ticket.Number,
                        i.Charge,
                        i.Ticket.Id
                    }))
                {
                    res1.Add(new TreatmentEvent
                    {
                        Id = Guid.Empty,
                        VisitDate = item.Date.Date,
                        EndTime = item.Date.Date,
                        SerializedTicketNumber = item.Number,
                        SerializedTreatmentTypeName = "Автоматическое списание",
                        VisitStatus = -1,
                        Price = item.Charge,
                        Cost = item.Charge,
                        TicketId = item.Id
                    });
                }
                res1 = res1.OrderBy(i => i.VisitDate).ToList();
#endif
                return res1;
            }
        }

        public static List<TextAction> GetCurrentActions(Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                return context.TextActions.Where(i => (!i.Divisions.Any() || i.Divisions.Any(j => j.Id == divisionId)) && i.StartDate <= DateTime.Today && i.FinishDate >= DateTime.Today).ToList();
            }
        }

        public static List<GoodSale> GetBarOrdersForCustomer(Guid customerId, DateTime startDate, DateTime endDate)
        {
            using(var context = new TonusEntities())
            {
                endDate = endDate.Date.AddDays(1);
                var res = context.GoodSales.Where(s => s.BarOrder.CustomerId == customerId && s.BarOrder.PurchaseDate >= startDate.Date && s.BarOrder.PurchaseDate <= endDate).ToList();
                res.ForEach(i => i.Init());
                return res;
            }
        }

        public static void PostCompany(Company company)
        {
            using(var context = new TonusEntities())
            {
                var orig = context.GetObjectByKey(new System.Data.EntityKey("TonusEntities.Companies", "CompanyId", company.CompanyId));

                context.Detach(orig);
                context.AttachTo("Companies", company);
                context.ObjectStateManager.ChangeObjectState(company, System.Data.EntityState.Modified);
                context.SaveChanges();
            }
        }

        public static Guid PostTreatmentProgram(TreatmentProgram treatmentProgram, List<Guid> lines)
        {
            treatmentProgram.IsAvail = true;
            var res = Core.PostEntities("TreatmentPrograms", new[] { treatmentProgram })[0];
            using(var context = new TonusEntities())
            {
                var prog = context.TreatmentPrograms.Single(i => i.Id == res);
                prog.TreatmentProgramLines.ToList().ForEach(i => context.DeleteObject(i));
                byte c = 1;
                lines.ForEach(line =>
                        prog.TreatmentProgramLines.Add(new TreatmentProgramLine
                        {
                            CompanyId = prog.CompanyId,
                            Id = Guid.NewGuid(),
                            Position = c++,
                            TreatmentConfigId = line,
                            TreatmentProgramId = res
                        }
                        ));
                context.SaveChanges();
                return res;
            }
        }

        public static List<Guid> GetCustomerStatusesIds(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                return context.Customers.Where(i => i.Id == customerId).SelectMany(i => i.CustomerStatuses).Select(i => i.Id).Distinct().ToList();
            }
        }

        public static void PostCustomerStatuses(Guid customerId, List<Guid> list)
        {
            using(var context = new TonusEntities())
            {
                var customer = context.Customers.Single(c => c.Id == customerId);

                customer.CustomerStatuses.Clear();

                foreach(var id in list)
                {
                    var stat = context.CustomerStatuses.Single(c => c.Id == id);
                    customer.CustomerStatuses.Add(stat);
                }
                context.SaveChanges();
                Logger.DBLog("Добавление/изменение Статусов клиента {0} ({1} шт.)", customer.FullName, list.Count);
            }
        }

        public static List<ChildrenRoom> GetCustomerChildren(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                var customer = context.GetObjectByKey(new EntityKey("TonusEntities.Customers", "Id", customerId)) as Customer;

                var res = customer.ChildrenRooms.ToList();
                res.ForEach(i => i.Init());
                return res;
            }
        }

        public static void PostDivision(Division division)
        {
            using(var context = new TonusEntities())
            {
                var orig = context.GetObjectByKey(new System.Data.EntityKey("TonusEntities.Divisions", "Id", division.Id));
                context.Detach(orig);
                context.AttachTo("Divisions", division);
                context.ObjectStateManager.ChangeObjectState(division, System.Data.EntityState.Modified);
                context.SaveChanges();
            }
        }

        public static List<int> GetAvailableShelfNumbers(Guid divisionId, bool isSafe)
        {
            using(var context = new TonusEntities())
            {
                var res = new List<int>();

                var div = context.GetObjectByKey(new System.Data.EntityKey("TonusEntities.Divisions", "Id", divisionId)) as Division;
                var shs = new string[0];
                if(isSafe)
                {
                    shs = (div.SafesRepository ?? "").Replace(" ", "").Split(',');
                }
                else
                {
                    shs = (div.ShelvesRepository ?? "").Replace(" ", "").Split(',');
                }
                var occupy = context.CustomerShelves
                    .Where(i => i.DivisionId == divisionId && !i.ReturnById.HasValue && i.IsSafe == isSafe)
                    .Select(i => i.ShelfNumber).Distinct().ToArray();
                foreach(var s in shs)
                {
                    int i;
                    if(Int32.TryParse(s, out i))
                    {
                        if(!occupy.Contains(i)) res.Add(i);
                    }
                }


                return res;

            }
        }

        public static List<CustomerShelf> GetCustomerShelves(Guid customerId, bool isSafe)
        {
            using(var context = new TonusEntities())
            {
                var res = context.CustomerShelves.Where(i => i.CustomerId == customerId && i.IsSafe == isSafe).ToList();
                res.ForEach(i => i.Init());
                return res;
            }
        }

        public static List<CustomerShelf> GetDivisionShelves(Guid divisionId, DateTime startPeriod, DateTime endPeriod, bool isSafe)
        {
            using(var context = new TonusEntities())
            {
                endPeriod = endPeriod.Date.AddDays(1);
                var res = context.CustomerShelves
                    .Include("Division")
                    .Include("CreatedBy")
                    .Include("ReturnBy")
                    .Include("Customer")
                    .Include("Customer.CustomerCards")
                    .Include("Customer.CustomerCards.CustomerCardType")
                    .Where(i => i.DivisionId == divisionId && ((i.CreatedOn >= startPeriod.Date && i.CreatedOn < endPeriod) || !i.ReturnOn.HasValue) && i.IsSafe == isSafe).ToList();
                res.ForEach(i => i.Init());
                return res;
            }
        }

        public static List<Solarium> GetDivisionSolariums(Guid divisionId, bool activeOnly)
        {
            using(var context = new TonusEntities())
            {
                var res = context.Solariums.Where(i => i.DivisionId == divisionId && (i.IsActive || !activeOnly)).OrderBy(i => i.Name).ToList();
                //res.ForEach(i => i.Init());
                return res;
            }
        }

        public static Dictionary<int, string> GetSolariumWarnings()
        {
            using(var context = new TonusEntities())
            {
                return context.SolariumMessages.OrderBy(i => i.MinMinutes).ToDictionary(i => i.MinMinutes, i => i.Message);
            }
        }

        public static Guid PostSolariumBooking(Guid divisionId, Guid customerId, Guid solariumId, DateTime dateTime, int amount, string comment)
        {
            using(var context = new TonusEntities())
            {
                var div = context.Divisions.Single(i => i.Id == divisionId);
                var sv = new SolariumVisit
                {
                    Amount = amount,
                    AuthorId = UserManagement.GetUser(context).UserId,
                    Comment = (comment ?? "").Trim(),
                    CompanyId = div.CompanyId,
                    CreatedOn = DateTime.Now,
                    CustomerId = customerId,
                    DivisionId = div.Id,
                    Id = Guid.NewGuid(),
                    SolariumId = solariumId,
                    Status = 0,
                    VisitDate = dateTime
                };
                context.SolariumVisits.AddObject(sv);
                context.SaveChanges();
                var cust = context.Customers.Where(i => i.Id == customerId).Select(i => i.LastName).FirstOrDefault();
                Logger.DBLog("Запись клиента в солярий. Клиент {0} Клуб {1} Солярий {2} Дата {3}", cust, div.Name, solariumId, dateTime);

                return sv.Id;
            }
        }

        public static Guid PostSolariumBookingEx(Guid divisionId, Guid customerId, Guid solariumId, DateTime dateTime, int amount, string comment, Guid? ticketId)
        {
            using(var context = new TonusEntities())
            {
                var div = context.Divisions.Single(i => i.Id == divisionId);

                if(ticketId.HasValue)
                {
                    var x = context.Tickets.Where(i => i.Id == ticketId).Select(i => i.SolariumMinutes).Single();
                    if(x > 0)
                    {
                        var t = context.SolariumVisits.Where(i => i.TicketId == ticketId && (i.Status == 2 || i.Status == 3 || i.Status == 0));
                        if(t.Any())
                        {
                            if(t.Sum(i => i.Amount) >= x)
                            {
                                ticketId = null;
                            }
                        }
                    }
                }

                var sv = new SolariumVisit
                {
                    Amount = amount,
                    AuthorId = UserManagement.GetUser(context).UserId,
                    Comment = (comment ?? "").Trim(),
                    CompanyId = div.CompanyId,
                    CreatedOn = DateTime.Now,
                    CustomerId = customerId,
                    DivisionId = div.Id,
                    Id = Guid.NewGuid(),
                    SolariumId = solariumId,
                    Status = 0,
                    VisitDate = dateTime,
                    TicketId = ticketId
                };
                context.SolariumVisits.AddObject(sv);
                context.SaveChanges();
                var cust = context.Customers.Where(i => i.Id == customerId).Select(i => i.LastName).FirstOrDefault();
                Logger.DBLog("Запись клиента в солярий. Клиент {0} Клуб {1} Солярий {2} Дата {3}", cust, div.Name, solariumId, dateTime, ticketId);

                return sv.Id;
            }
        }

        public static KeyValuePair<Guid, DateTime> GetSolariumProposal(Guid divisionId, Guid customerId, DateTime dateTime, int amount, Guid selectedSolariumId, Guid toSkip)
        {
            using(var context = new TonusEntities())
            {
                if(selectedSolariumId != Guid.Empty)
                {
                    var sol = context.Solariums.Single(i => i.Id == selectedSolariumId);
                    if(!sol.IsActive)
                    {
                        var str0 = Localization.Resources.NoBookingAvail;
                        throw new FaultException<string>(str0, str0);
                    }
                    var time = dateTime;
                    var date = dateTime.Date;
                    var date1 = date.AddDays(1);
                    var plan = context.SolariumVisits.Where(i => i.VisitDate >= date && i.VisitDate < date1 && i.Status != 1 && (i.SolariumId == selectedSolariumId || i.CustomerId == customerId) && i.Id != toSkip).OrderBy(i => i.VisitDate);
                    bool flag;
                    do
                    {
                        flag = true;
                        foreach(var p in plan)
                        {
                            if(Core.DatesIntersects(time, time.AddMinutes(amount + p.Solarium.MaintenaceTime), p.VisitDate, p.VisitDate.AddMinutes(p.Amount + p.Solarium.MaintenaceTime)))
                            {
                                flag = false;
                                break;
                            }
                        }
                        if(!flag)
                        {
                            time = time.AddMinutes(1);
                        }
                    } while(!flag);
                    return new KeyValuePair<Guid, DateTime>(selectedSolariumId, time);
                }
                else
                {
                    var time = DateTime.MaxValue;
                    var solId = Guid.Empty;
                    foreach(var i in context.Solariums.Where(i => i.IsActive && i.DivisionId == divisionId))
                    {
                        try
                        {
                            var j = GetSolariumProposal(divisionId, customerId, dateTime, amount, i.Id, toSkip);
                            if(j.Value < time)
                            {
                                time = j.Value;
                                solId = j.Key;
                            }
                        }
                        catch { }
                    }
                    if(solId != Guid.Empty)
                    {
                        return new KeyValuePair<Guid, DateTime>(solId, time);
                    }
                }
                var str = Localization.Resources.NoBookingAvail;
                throw new FaultException<string>(str, str);
            }
        }

        public static List<SolariumVisit> GetCustomerSolarium(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                var res = context.SolariumVisits.Where(i => i.CustomerId == customerId).ToList();
                res.ForEach(i => i.Init());
                return res;
            }
        }

        public static List<SolariumVisit> GetDivisionSolariumVisits(Guid divisionId, DateTime startDate, DateTime finishDate)
        {
            using(var context = new TonusEntities())
            {
                var res = context.SolariumVisits.Where(i => i.DivisionId == divisionId && i.VisitDate >= startDate && i.VisitDate <= finishDate).ToList();
                res.ForEach(i => i.Init());
                return res;
            }
        }

        public static bool CancelSolariumEvent(Guid solVisitId, bool delete)
        {

            using(var context = new TonusEntities())
            {
                var sv = context.SolariumVisits.Single(i => i.Id == solVisitId);
                Logger.DBLog("Отмена записи в солярий ({0})", sv.VisitDate);
                if(sv.Cost.HasValue) return false;
                if(delete)
                {
                    context.DeleteObject(sv);
                }
                else
                {
                    sv.eStatus = SolariumVisitStatus.Canceled;
                }
                context.SaveChanges();
                return true;
            }
        }

        public static List<Ticket> GetCustomerTickets(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                var res = context.Tickets.Where(i => i.CustomerId == customerId).ToList();
                res.ForEach(i => i.InitDetails());
                return res;
            }
        }

        public static void PostSolariumVisitStart(Guid solariumVisitId)
        {
            using(var context = new TonusEntities())
            {
                var sv = context.SolariumVisits.Single(i => i.Id == solariumVisitId);
                Ticket ticket = null;

                if(sv.eStatus == SolariumVisitStatus.Completed) return;

                sv.eStatus = SolariumVisitStatus.Completed;

                if(sv.TicketId.HasValue)
                {
                    ticket = context.Tickets.Single(i => i.Id == sv.TicketId);
                    ticket.InitDetails();
                    if(ticket.SolariumMinutesLeft >= sv.Amount)
                    {
                        if(!ticket.IsActive)
                        {
                            ActivateTicket(ticket.Id);
                        }
                    }
                    else
                    {
                        ticket = null;
                    }
                }
                if(ticket == null)
                {
                    //Списываем с первого попавшегося активного абонемента минуты
                    var tickets = context.Tickets.Where(i => i.CustomerId == sv.CustomerId && i.IsActive == true).ToList();
                    tickets.ForEach(i => i.InitDetails());
                    ticket = tickets.FirstOrDefault(i => i.SolariumMinutesLeft >= sv.Amount);
                    //Если не находим - активируем новый
                    if(ticket == null)
                    {
                        tickets = context.Tickets.Where(i => i.CustomerId == sv.CustomerId && i.IsActive == false).ToList();
                        tickets.ForEach(i => i.InitDetails());
                        ticket = tickets.First(i => i.SolariumMinutesLeft >= sv.Amount);
                        ActivateTicket(ticket.Id);
                    }
                }

                var mc = new MinutesCharge
                {
                    AuthorId = UserManagement.GetUser(context).UserId,
                    CompanyId = sv.CompanyId,
                    Date = DateTime.Now,
                    Id = Guid.NewGuid(),
                    MinutesCharged = sv.Amount * sv.Solarium.TicketMinutePrice,
                    Reason = String.Format(Localization.Resources.SolChargeText, sv.Solarium.Name, sv.VisitDate),
                    TicketId = ticket.Id
                };
                sv.TicketId = ticket.Id;

                context.MinutesCharges.AddObject(mc);
                context.SaveChanges();
            }
        }

        public static void PostSolariumWarnings(List<KeyValuePair<int, string>> solariumWarnings)
        {
            using(var context = new TonusEntities())
            {
                var ws = context.SolariumMessages.ToList();
                ws.ForEach(i => context.DeleteObject(i));

                solariumWarnings.ForEach(i =>
                    context.SolariumMessages.AddObject(new SolariumMessage { Id = Guid.NewGuid(), MinMinutes = i.Key, Message = i.Value }));

                context.SaveChanges();
            }
        }

        public static void PostCompanyCardTypeEnable(Guid ccardId, bool enable)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                var ccard = context.CustomerCardTypes.Single(i => i.Id == ccardId);
                if(enable)
                {
                    if(!user.Company.CustomerCardTypes.Any(i => i.Id == ccardId))
                    {
                        user.Company.CustomerCardTypes.Add(ccard);
                    }
                }
                else
                {
                    if(user.Company.CustomerCardTypes.Any(i => i.Id == ccardId))
                    {
                        user.Company.CustomerCardTypes.Remove(ccard);
                    }
                }

                context.SaveChanges();
            }
        }

        public static void PostCompanyTicketTypeEnable(Guid tTypeId, bool enable)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                var tType = context.TicketTypes.Single(i => i.Id == tTypeId);
                if(enable)
                {
                    if(!user.Company.TicketTypes.Any(i => i.Id == tTypeId))
                    {
                        user.Company.TicketTypes.Add(tType);
                    }
                }
                else
                {
                    if(user.Company.TicketTypes.Any(i => i.Id == tTypeId))
                    {
                        user.Company.TicketTypes.Remove(tType);
                    }
                }

                context.SaveChanges();
            }
        }

        public static List<ProviderFolder> GetProviderFolders()
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                return context.ProviderFolders.Where(i => i.CompanyId == user.CompanyId).ToList();
            }
        }

        public static void PostProviderFolder(ProviderFolder providerFolder)
        {
            //TODO: test that division is right
            if(providerFolder.ParentFolderId == Guid.Empty) providerFolder.ParentFolderId = null;
            Core.PostEntities("ProviderFolders", new[] { providerFolder });
        }

        public static void DeleteProviderFolder(Guid folderId)
        {
            using(var context = new TonusEntities())
            {
                var fld = context.ProviderFolders.SingleOrDefault(i => i.Id == folderId);
                if(fld == null) return;

                fld.Providers.ToList().ForEach(i => i.ProviderFolderId = fld.ParentFolderId);
                fld.Providers.Clear();

                fld.ChildFolders.ToList().ForEach(i => i.ParentFolderId = fld.ParentFolderId);
                fld.ChildFolders.Clear();

                context.DeleteObject(fld);

                context.SaveChanges();
            }
        }

        public static void HideProvider(Guid providerId)
        {
            using(var context = new TonusEntities())
            {
                var provider = context.Providers.Single(i => i.Id == providerId);
                provider.IsVisible = false;

                context.SaveChanges();
            }
        }

        public static List<Provider> GetAllProviders()
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var res = context.Providers.Where(i => i.CompanyId == user.CompanyId && i.IsVisible).ToList();
                res.ForEach(i => i.Init());

                return res;
            }
        }

        public static void HideGood(Guid goodId)
        {
            using(var context = new TonusEntities())
            {
                var good = context.Goods.Single(i => i.Id == goodId);
                good.IsVisible = false;
                Logger.DBLog("Скрытие товара {0}", good.Name);
                context.SaveChanges();
            }
        }

        public static List<Good> GetAllGoods(Guid companyId)
        {
            using(var context = new TonusEntities())
            {
                return context.Goods.Where(i => i.IsVisible && i.CompanyId == companyId).OrderBy(i => i.Name).ToList().Init();
            }
        }

        public static List<Storehouse> GetStorehouses(Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                var res = context.Storehouses.Where(i => i.DivisionId == divisionId).ToList();
                //res.ForEach(i => i.Init());

                return res;
            }
        }

        public static Guid PostConsignment(Consignment consignment)
        {
            using(var context = new TonusEntities())
            {
                if(consignment.Number == 0)
                {
                    var year = DateTime.Today.Year;
                    var user = UserManagement.GetUser(context);
                    consignment.CompanyId = user.CompanyId;
                    consignment.AuthorId = user.UserId;
                    var other = context.Consignments.Where(i => i.Date.Year == year && i.DocType == consignment.DocType && i.CompanyId == consignment.CompanyId);
                    consignment.Number = other.Max(i => i.Number, 0) + 1;
                }
                return Core.PostEntities("Consignments", new[] { consignment })[0];
            }
        }

        public static List<ChildrenRoom> GetDivisionChildren(Guid divisionId, DateTime start, DateTime end)
        {
            using(var context = new TonusEntities())
            {
                end = end.Date.AddDays(1);
                var res = context.ChildrenRooms.Where(i => i.DivisionId == divisionId && i.CreatedOn >= start.Date && i.CreatedOn < end).ToList();
                return res.Init();
            }
        }

        public static List<GoodSale> GetDivisionSales(Guid divisionId, DateTime start, DateTime end)
        {
            using(var context = new TonusEntities())
            {
                end = end.Date.AddDays(1);
                var res = context.GoodSales
                    .Where(i => i.Storehouse.DivisionId == divisionId && i.BarOrder.PurchaseDate >= start && i.BarOrder.PurchaseDate < end && (i.BarOrder.Kind1C ?? -1) != 10)
                    .OrderByDescending(i => i.BarOrder.PurchaseDate).ToList().Init();
                context.Certificates.Where(ss => ss.SellDate >= start && ss.SellDate < end && ss.BuyerId.HasValue && ss.DivisionId == divisionId)
                    .ToList().ForEach(ss =>
                    {
                        var gs = new GoodSale
                        {
                            Amount = 1,
                            PriceBonus = ss.IsBonusSell ? ss.PriceBonus : null,
                            PriceMoney = ss.IsBonusSell ? (decimal?)null : (ss.PriceMoney ?? ss.Amount),
                            SerializedOrderNumber = ss.SellBarOrder.OrderNumber,
                            SerializedOrderDate = ss.SellBarOrder.PurchaseDate,
                            SerializedUnitType = Localization.Resources.items,
                            SerializedCustomer = ss.Customer.FullName,
                            SerializedCreatedBy = ss.CreatedBy.FullName,
                            SerializedGoodName = String.IsNullOrWhiteSpace(ss.Name) ? String.Format(Localization.Resources.CertFor, ss.Amount) : ss.Name,
                            BarOrder = ss.SellBarOrder
                        };
                        gs.InitPaymentWay();
                        res.Add(gs);
                    });
                var bos = context.BarOrders.Where(i => i.DivisionId == divisionId && i.PurchaseDate >= start && i.PurchaseDate < end && i.Kind1C == 10)
                    .OrderByDescending(i => i.PurchaseDate).ToList().Init();
                foreach(var bo in bos)
                {
                    var list = bo.GetContent();
                    foreach(var l in list)
                    {
                        var gs = new GoodSale
                        {
                            Amount = l.InBasket,
                            PriceBonus = null,
                            PriceMoney = l.Price,
                            SerializedOrderNumber = bo.OrderNumber,
                            SerializedOrderDate = bo.PurchaseDate,
                            SerializedUnitType = l.UnitName,
                            SerializedCustomer = bo.Customer.FullName,
                            SerializedCreatedBy = bo.CreatedBy.FullName,
                            SerializedGoodName = l.Name,
                            BarOrder = bo
                        };
                        gs.InitPaymentWay();
                        res.Add(gs);
                    }
                }
                return res;
            }
        }

        public static List<Consignment> GetAllConsignments(Guid divisionId, DateTime start, DateTime end, bool cons)
        {
            using(var context = new TonusEntities())
            {
                end = end.AddDays(1);
                var res = context.Consignments.Where(i => (i.Comment == null || i.Comment != "#Deleted#") && i.DivisionId == divisionId && i.Date >= start && i.Date < end && ((i.DocType == 3 && !cons) || (i.DocType != 3 && cons))).OrderByDescending(i => i.Date).ToList().Init();
                return res;
            }
        }

        public static List<ProviderPayment> GetAllProviderPayments(Guid divisionId, DateTime start, DateTime end)
        {
            using(var context = new TonusEntities())
            {
                end = end.Date.AddDays(1);
                return context.ProviderPayments.Where(i => i.Order.DivisionId == divisionId && i.Date >= start && i.Date < end).ToList().Init();
            }
        }

        public static List<GoodSale> GetCustomerSales(Guid customerId, DateTime start, DateTime end)
        {
            using(var context = new TonusEntities())
            {
                start = start.Date;
                end = end.Date.AddDays(1);

                var result = context.GoodSales
                    .Include("Good")
                    .Include("Good.UnitType")
                    .Include("BarOrder")
                    .Include("Storehouse")
                    .Include("BarOrder.Customer")
                    .Where(i => i.BarOrder.CustomerId == customerId && i.BarOrder.PurchaseDate >= start && i.BarOrder.PurchaseDate < end && ((i.BarOrder.Kind1C ?? -1) != 10)).ToList().Init();

                context.Certificates.Where(i => i.BuyerId == customerId).ToList().ForEach(ss =>
                    {
                        var gs = new GoodSale
                        {
                            Amount = 1,
                            PriceBonus = ss.IsBonusSell ? ss.PriceBonus : null,
                            PriceMoney = ss.IsBonusSell ? (decimal?)null : (ss.PriceMoney ?? ss.Amount),
                            SerializedOrderNumber = ss.SellBarOrder.OrderNumber,
                            SerializedOrderDate = ss.SellBarOrder.PurchaseDate,
                            SerializedUnitType = "шт.",
                            SerializedCustomer = ss.Customer.FullName,
                            SerializedCreatedBy = ss.CreatedBy.FullName,
                            SerializedGoodName = String.IsNullOrWhiteSpace(ss.Name) ? String.Format(Localization.Resources.CertFor, ss.Amount) : ss.Name,
                            BarOrder = ss.SellBarOrder
                        };
                        gs.InitPaymentWay();
                        result.Add(gs);
                    });

                var bos = context.BarOrders.Where(i => i.CustomerId == customerId && i.PurchaseDate >= start && i.PurchaseDate < end && i.Kind1C == 10)
                    .OrderByDescending(i => i.PurchaseDate).ToList().Init();
                foreach(var bo in bos)
                {
                    var list = bo.GetContent();
                    foreach(var l in list)
                    {
                        var gs = new GoodSale
                        {
                            Amount = 1,
                            PriceBonus = null,
                            PriceMoney = l.Price,
                            SerializedOrderNumber = bo.OrderNumber,
                            SerializedOrderDate = bo.PurchaseDate,
                            SerializedUnitType = l.UnitName,
                            SerializedCustomer = bo.Customer.FullName,
                            SerializedCreatedBy = bo.CreatedBy.FullName,
                            SerializedGoodName = l.Name,
                            BarOrder = bo
                        };
                        gs.InitPaymentWay();
                        result.Add(gs);
                    }
                }

                return result;
            }
        }

        public static List<Certificate> GetDivisionCertificates(Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                return context.Certificates.Where(i => i.DivisionId == divisionId).ToList().Init();
            }
        }

        public static void PostCertificate(Certificate certificate)
        {
            if(certificate.PriceBonus == 0) certificate.PriceBonus = null;
            if(certificate.PriceMoney == 0) certificate.PriceMoney = null;

            Logger.DBLog("Добавление/изменение сертификата {0}", certificate.PriceMoney);

            PostEntities("Certificates", new[] { certificate });
        }

        public static bool CancelCertificate(Guid certificateId)
        {
            using(var context = new TonusEntities())
            {
                var cert = context.Certificates.SingleOrDefault(i => i.Id == certificateId);
                if(cert == null) return false;
                if(cert.SerializedUseDate.HasValue || cert.BuyerId.HasValue) return false;
                context.DeleteObject(cert);
                context.SaveChanges();
                return true;
            }
        }

        public static List<Rent> GetCustomerRent(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                return context.Rents.Where(i => i.CustomerId == customerId).OrderBy(i => i.CreatedOn).ToList().Init();
            }
        }

        public static Certificate GetCertificateByNumber(int id)
        {
            using(var context = new TonusEntities())
            {
                var str = id.ToString();
                var cId = UserManagement.GetUser(context).CompanyId;
                var res = context.Certificates.SingleOrDefault(i => i.CompanyId == cId && i.BarCode == str && i.BuyerId.HasValue && !i.UsedOrderId.HasValue);
                if(res != null)
                {
                    res.Init();
                }
                return res;
            }
        }

        public static List<BarOrder> GetDivisionBarOrders(Guid divisionId, DateTime start, DateTime end)
        {
            using(var context = new TonusEntities())
            {
                start = start.Date;
                end = end.Date.AddDays(1);
                return context.BarOrders.Where(i => i.DivisionId == divisionId && i.PurchaseDate >= start && i.PurchaseDate < end && i.OrderNumber > 0)
                    .OrderBy(i => i.OrderNumber).ToList().Init();
            }
        }

        public static List<DepositAccount> GetCustomerDeposit(Guid customerId, DateTime start, DateTime end)
        {
            using(var context = new TonusEntities())
            {
                start = start.Date;
                end = end.Date.AddDays(1);
                return context.DepositAccounts.Where(i => i.CustomerId == customerId && i.CreatedOn >= start && i.CreatedOn < end).OrderByDescending(i => i.CreatedOn).ToList().Init();
            }
        }

        public static void PostDepositAdd(Guid customerId, decimal amount, string description)
        {
            using(var context = new TonusEntities())
            {
                var customer = context.Customers.Single(i => i.Id == customerId);
                var user = UserManagement.GetUser(context);
                customer.InitActiveCard();
                if(customer.ActiveCard != null)
                {
                    amount *= (1 + customer.ActiveCard.CustomerCardType.BonusPercent);
                }

                var da = new DepositAccount
                {
                    Amount = amount,
                    AuthorId = user.UserId,
                    CompanyId = customer.CompanyId,
                    CreatedOn = DateTime.Now,
                    CustomerId = customer.Id,
                    Description = Localization.Resources.Deposit,
                    Id = Guid.NewGuid()
                };
                context.DepositAccounts.AddObject(da);
                context.SaveChanges();
                Logger.DBLog("Пополнение депозита клиенту {0} на сумму {1}", customer.FullName, amount);

            }
        }

        public static Guid RequestDepositOut(Guid customerId, decimal amount)
        {
            using(var context = new TonusEntities())
            {
                var customer = context.Customers.Single(i => i.Id == customerId);
                var d = new DepositOut
                {
                    Amount = amount,
                    AuthorId = UserManagement.GetUser(context).UserId,
                    CompanyId = customer.CompanyId,
                    CreatedOn = DateTime.Now,
                    CustomerId = customerId,
                    Id = Guid.NewGuid()
                };
                context.DepositOuts.AddObject(d);
                context.SaveChanges();
                Logger.DBLog("Запрос на вывод с депозита по клиенту {0} на сумму {1}", customer.FullName, amount);

                return d.Id;
            }
        }

        public static void PostDepositOutDone(Guid depositOutId, string comment, bool isDone)
        {
            using(var context = new TonusEntities())
            {
                var dout = context.DepositOuts.Include("Company")
                    .Single(i => i.Id == depositOutId);
                dout.Init();

                var user = UserManagement.GetUser(context);
                dout.Comment = comment;
                dout.ProcessedById = user.UserId;
                dout.ProcessedOn = DateTime.Now;

                if(isDone)
                {
                    var currentUserBalance =
                        context.DepositAccounts.Where(i => i.CustomerId == dout.CustomerId).Sum(i => (decimal?) i.Amount) ?? 0;

                    if (currentUserBalance < dout.Amount)
                    {
                        throw new FaultException<string>("У клиента недостаточно средств для вывода!", "У клиента недостаточно средств для вывода!");
                    }

                    var da = new DepositAccount
                    {
                        Amount = -dout.Amount,
                        AuthorId = user.UserId,
                        CompanyId = dout.CompanyId,
                        CreatedOn = DateTime.Now,
                        CustomerId = dout.CustomerId,
                        Description = String.Format(Localization.Resources.Withdrawal, dout.TotalAmount),
                        Id = Guid.NewGuid()
                    };
                    context.DepositAccounts.AddObject(da);
                }

                context.SaveChanges();
                Logger.DBLog("Вывод с депозита {0} ({1})", dout.Customer.FullName, dout.Amount);

            }
        }

        public static List<Spending> GetDivisionSpendings(Guid divisionId, DateTime start, DateTime end)
        {
            using(var context = new TonusEntities())
            {
                var cId = UserManagement.GetUser(context).CompanyId;
                start = start.Date;
                end = end.Date.AddDays(1);
                return context.Spendings.Where(i => (i.DivisionId == divisionId || (!i.DivisionId.HasValue && i.CompanyId == cId))
                    && i.CreatedOn >= start && i.CreatedOn < end).OrderByDescending(i => i.Number).ToList().Init();
            }
        }

        public static List<SpendingType> GetDivisionSpendingTypes(Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                return context.SpendingTypes.Where(i => i.DivisionId == divisionId).Where(i => !i.IsDeleted).OrderBy(i => i.Name).ToList();
            }
        }

        public static void PostSpending(Spending spending)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var old = context.Spendings.SingleOrDefault(i => i.Id == spending.Id);
                if(old == null)
                {
                    spending.Id = Guid.NewGuid();
                    spending.AuthorId = user.UserId;
                    spending.Number = context.Spendings.Where(i => i.DivisionId == spending.DivisionId).Max(i => i.Number, 0) + 1;
                    if(spending.IsFinAction)
                    {
                        spending.DivisionId = null;
                        spending.IsInvestment = false;
                    }
                    context.Spendings.AddObject(spending);
                }
                else
                {
                    old.Amount = spending.Amount;
                    old.CreatedOn = spending.CreatedOn;
                    old.SpendingTypeId = spending.SpendingTypeId;
                    old.PaymentType = spending.PaymentType;
                    old.DivisionId = spending.DivisionId;
                    old.IsInvestment = spending.IsInvestment;
                    old.Name = spending.Name;
                    old.IsFinAction = spending.IsFinAction;
                    if(old.IsFinAction)
                    {
                        old.DivisionId = null;
                        old.IsInvestment = false;
                    }
                }
                context.SaveChanges();
            }
        }

        public static void PostSpendingType(Guid divisionId, Guid typeId, string name)
        {
            using(var context = new TonusEntities())
            {
                var division = context.Divisions.Single(i => i.Id == divisionId);
                SpendingType st;
                if(typeId == Guid.Empty)
                {
                    st = new SpendingType
                    {
                        CompanyId = division.CompanyId,
                        DivisionId = division.Id,
                        Id = Guid.NewGuid(),
                        IsCommon = false,
                        Name = name.Trim()
                    };
                    context.SpendingTypes.AddObject(st);
                }
                else
                {
                    st = context.SpendingTypes.Single(i => i.Id == typeId);
                    st.Name = name.Trim();
                }
                context.SaveChanges();
            }
        }

        public static List<BonusAccount> GetCustomerBonus(Guid customerId, DateTime start, DateTime end)
        {
            using(var context = new TonusEntities())
            {
                start = start.Date;
                end = end.Date.AddDays(1);
                return context.BonusAccounts.Where(i => i.CustomerId == customerId && i.CreatedOn >= start && i.CreatedOn < end).OrderByDescending(i => i.CreatedOn).ToList().Init();
            }
        }

#if BEAUTINIKA
        public static void PostLastTicketCorrection(Guid customerId, DateTime corrActivated, int corrAmount, int corrExtra, int corrGuest, decimal paidAmt, int solCorr)
#else
        public static void PostLastTicketCorrection(Guid customerId, DateTime corrActivated, int corrAmount, int corrGuest, decimal paidAmt, int solCorr)
#endif
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var ticket = context.Tickets.Where(i => i.CustomerId == customerId).OrderByDescending(i => i.CreatedOn).First();
                ticket.IsActive = true;
                ticket.StartDate = corrActivated;
                ticket.UnitsAmount = corrAmount;
#if BEAUTINIKA
                ticket.ExtraUnitsAmount = corrExtra;
#endif
                ticket.GuestUnitsAmount = corrGuest;
                ticket.SolariumMinutes = solCorr;

                if(paidAmt > 0m)
                {
                    var pmt = new TicketPayment
                    {
                        Amount = paidAmt,
                        AuthorId = user.UserId,
                        CompanyId = ticket.CompanyId,
                        Id = Guid.NewGuid(),
                        PaymentDate = corrActivated,
                        TicketId = ticket.Id,
                        ReceiptNumber = GetNewReceiptNumber(user.CompanyId)
                    };
                    context.TicketPayments.AddObject(pmt);
                }

                if(ticket.InstalmentId.HasValue)
                {
                    ticket.LastInstalmentDay = ticket.StartDate.Value.AddDays(ticket.Instalment.Length);
                }

                context.SaveChanges();
                Logger.DBLog("Коррекция последнего абонемента клиента {0}", ticket.Customer.FullName);

            }
        }

        internal static int GetNewReceiptNumber(Guid companyId)
        {
            using(var context = new TonusEntities())
            {
                return (context.TicketPayments.Any(i => i.CompanyId == companyId) ? context.TicketPayments.Where(i => i.CompanyId == companyId).Max(i => i.ReceiptNumber ?? 0) : 0) + 1;
            }
        }

        public static List<Corporate> GetCorporates(Guid companyId)
        {
            using(var context = new TonusEntities())
            {
                return context.Corporates.Where(i => i.CompanyId == companyId && i.IsAvail).OrderBy(i => i.Name).ToList();
            }
        }

        public static bool PostCorporate(Guid companyId, Guid corpId, string name, Guid? folderId)
        {
            using(var context = new TonusEntities())
            {
                if(corpId == Guid.Empty)
                {
                    var x = context.Corporates.FirstOrDefault(i => i.CompanyId == companyId && i.Name.ToLower() == name.ToLower());
                    if(x != null) return false;
                    var corp = new Corporate
                    {
                        CompanyId = companyId,
                        Id = Guid.NewGuid(),
                        Name = name,
                        IsAvail = true,
                        SettingsFolderId = folderId
                    };
                    context.Corporates.AddObject(corp);
                }
                else
                {
                    var x = context.Corporates.FirstOrDefault(i => i.CompanyId == companyId && i.Name.ToLower() == name.ToLower() && i.Id != corpId);
                    if(x != null) return false;
                    var corp = context.Corporates.Single(i => i.Id == corpId);
                    corp.Name = name;
                    corp.SettingsFolderId = folderId;
                }
                context.SaveChanges();
                return true;
            }
        }

        public static bool DeleteCorporate(Guid corpId)
        {
            using(var context = new TonusEntities())
            {
                var corp = context.Corporates.Single(i => i.Id == corpId);
                foreach(var c in corp.Customers.ToList())
                {
                    c.CorporateId = null;
                }
                corp.IsAvail = false;
                context.SaveChanges();
                return true;
            }
        }

        public static List<Treatment> GetAllTreatments(Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                try
                {
                    return context.Treatments.Where(i => i.DivisionId == divisionId).OrderBy(i => i.Order).ToList().Init();
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
        }

        public static void PostIncomingCallForm(IncomingCallForm incomingCallForm)
        {
            using(var context = new TonusEntities())
            {

                if(incomingCallForm.Id == Guid.Empty)
                {
                    var form = new IncomingCallForm
                    {
                        Id = Guid.NewGuid(),
                        FormText = incomingCallForm.FormText,
                        HasInputBox = incomingCallForm.HasInputBox,
                        Header = incomingCallForm.Header,
                        IsStartForm = incomingCallForm.IsStartForm
                    };
                    context.IncomingCallForms.AddObject(form);
                    foreach(var btn in incomingCallForm.SerializedIncomingCallFormButtons)
                    {
                        var b = new IncomingCallFormButton
                        {
                            ButtonAction = btn.ButtonAction,
                            ButtonText = btn.ButtonText,
                            Id = Guid.NewGuid(),
                            IncomingCallFormId = form.Id,
                            Parameter = btn.Parameter,
                            IsFinal = btn.IsFinal
                        };
                        context.IncomingCallFormButtons.AddObject(b);
                    }
                }
                else
                {
                    var form = context.IncomingCallForms.Single(i => i.Id == incomingCallForm.Id);
                    foreach(var i in form.IncomingCallFormButtons.ToList())
                    {
                        context.DeleteObject(i);
                    }

                    context.Detach(form);
                    context.IncomingCallForms.Attach(incomingCallForm);
                    context.ObjectStateManager.ChangeObjectState(incomingCallForm, EntityState.Modified);
                    foreach(var btn in incomingCallForm.SerializedIncomingCallFormButtons)
                    {
                        var b = new IncomingCallFormButton
                        {
                            ButtonAction = btn.ButtonAction,
                            ButtonText = btn.ButtonText,
                            Id = Guid.NewGuid(),
                            IncomingCallFormId = form.Id,
                            Parameter = btn.Parameter,
                            IsFinal = btn.IsFinal
                        };
                        context.IncomingCallFormButtons.AddObject(b);
                    }
                }
                context.SaveChanges();
            }
        }

        public static List<Customer> GetCustomers(Func<Customer, bool> query)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var res = context.Customers.Where(c => c.CompanyId == user.CompanyId).Where(query).ToList();
                res.ForEach(i => i.InitActiveCard());
                return res;
            }
        }

        public static List<Customer> GetCustomersByStatus(List<Guid> statIds)
        {
            using(var context = new TonusEntities())
            {
                var res = context.CustomerStatuses.ToList().Where(i => statIds.Contains(i.Id)).SelectMany(i => i.Customers).Distinct().ToList();
                res.ForEach(i => i.InitActiveCard());
                return res;
            }
        }

        public static List<Customer> GetCustomersByManagers(List<Guid> managerIds)
        {
            using (var context = new TonusEntities())
            {
                var manIds = context.Users.Where(i => managerIds.Contains(i.UserId) && i.EmployeeId.HasValue).Select(i => i.EmployeeId).ToList();
                var res = context.Customers.Where(i => manIds.Contains(i.ManagerId)).Distinct().ToList();
                res.ForEach(i => i.InitActiveCard());
                return res;
            }
        }

        public static void PostReturnReject(Guid ticketId)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                var ticket = context.Tickets.Single(i => i.Id == ticketId);
                ticket.TicketFreezes.Where(i => i.TicketFreezeReason.IsReturnReason).ToList().ForEach(i =>
                {
                    context.DeleteObject(i);
                });
                Logger.DBLog("Отказ возврата абонемента {0}", ticket.Number);


                var task = new Task
                {
                    AuthorId = user.UserId,
                    ClosedById = user.UserId,
                    ClosedComment = Localization.Resources.RefundRefuse,
                    ClosedOn = DateTime.Now,
                    CompanyId = ticket.CompanyId,
                    CreatedOn = DateTime.Now,
                    ExpiryOn = DateTime.Now,
                    Id = Guid.NewGuid(),
                    Message = Localization.Resources.RefundNum + ticket.Number,
                    Priority = 1,
                    StatusId = 2,
                    Subject = Localization.Resources.RefundSub
                };
                context.Tasks.AddObject(task);

                context.SaveChanges();
            }
        }

        public static List<Guid> GetEmployeeIdsWithPermission(Guid divisionId, string permissionName)
        {
            using(var context = new TonusEntities())
            {
                var div = context.Divisions.Single(i => i.Id == divisionId);
                return UserManagement.GetEmployeesWithPermission(new TonusEntities(), div.CompanyId, divisionId, permissionName).Select(i => i.Id).ToList();
            }
        }

        public static List<Division> GetDivisions()
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                return context.Divisions.Where(i => i.CompanyId == user.CompanyId).ToList();
            }
        }

        public static void PostInstalmentDelete(Guid instalmentId)
        {
            using(var context = new TonusEntities())
            {
                var inst = context.Instalments.SingleOrDefault(i => i.Id == instalmentId);
                inst.IsActive = false;
                context.SaveChanges();
            }
        }

        public static void PostInstalment(Instalment instalment)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                Instalment inst;
                if(instalment.Id != Guid.Empty)
                {
                    inst = context.Instalments.SingleOrDefault(i => i.Id == instalment.Id);
                }
                else
                {
                    inst = new Instalment
                    {
                        AuthorId = user.UserId,
                        //CompanyId = user.CompanyId,
                        Id = Guid.NewGuid(),
                        IsActive = true
                    };
                    context.Instalments.AddObject(inst);
                }
                inst.Name = instalment.Name.Trim();
                inst.AvailableUnitsPercent = instalment.AvailableUnitsPercent;
                inst.ContribAmount = instalment.ContribAmount;
                inst.ContribPercent = instalment.ContribPercent;
                inst.Length = instalment.Length;
                inst.SettingsFolderId = instalment.SettingsFolderId;
                inst.SecondLength = instalment.SecondLength;
                inst.SecondPercent = instalment.SecondPercent;
                context.SaveChanges();
            }
        }

        public static List<CompanyFinance> GetCompanyFinances(Guid companyId)
        {
            using(var context = new TonusEntities())
            {
                return context.CompanyFinances.Where(i => i.CompanyId == companyId).OrderByDescending(i => i.Period).ToList();
            }
        }

        public static void PostCompanyFinance(Guid companyId, DateTime period, decimal accLeft)
        {
            using(var context = new TonusEntities())
            {
                var fin = context.CompanyFinances.SingleOrDefault(i => i.CompanyId == companyId && i.Period == period);
                if(fin == null)
                {
                    fin = new CompanyFinance { Id = Guid.NewGuid(), CompanyId = companyId, Period = period };
                    context.CompanyFinances.AddObject(fin);
                }
                fin.AccountLeft = accLeft;
                context.SaveChanges();
            }
        }

        public static void PostDivisionFinance(Guid divisionId, DateTime period, decimal cash, decimal unsent, decimal advances, decimal loan, decimal accumulated)
        {
            using(var context = new TonusEntities())
            {
                var div = context.Divisions.SingleOrDefault(i => i.Id == divisionId);
                var fin = context.DivisionFinances.SingleOrDefault(i => i.DivisionId == divisionId && i.Period == period);
                if(fin == null)
                {
                    fin = new DivisionFinance { Id = Guid.NewGuid(), CompanyId = div.CompanyId, DivisionId = divisionId, Period = period };
                    context.DivisionFinances.AddObject(fin);
                }
                fin.CashLeft = cash;
                fin.Unsent = unsent;
                fin.Advances = advances;
                fin.TerminalLoan = loan;
                fin.Accum = accumulated;
                context.SaveChanges();
            }
        }

        public static List<DivisionFinance> GetDivisionFinances(Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                return context.DivisionFinances.Where(i => i.DivisionId == divisionId).OrderByDescending(i => i.Period).ToList();
            }
        }

        public static List<IncomeType> GetDivisionIncomeTypes(Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                return context.IncomeTypes.Where(i => i.DivisionId == divisionId).Where(i => !i.IsDeleted).OrderBy(i => i.Name).ToList();
            }
        }

        public static void PostIncomeType(Guid divisionId, Guid typeId, string name)
        {
            using(var context = new TonusEntities())
            {
                var division = context.Divisions.Single(i => i.Id == divisionId);
                IncomeType st;
                if(typeId == Guid.Empty)
                {
                    st = new IncomeType
                    {
                        CompanyId = division.CompanyId,
                        DivisionId = division.Id,
                        Id = Guid.NewGuid(),
                        IsCommon = false,
                        Name = name.Trim()
                    };
                    context.IncomeTypes.AddObject(st);
                }
                else
                {
                    st = context.IncomeTypes.Single(i => i.Id == typeId);
                    st.Name = name.Trim();
                }
                context.SaveChanges();
            }
        }

        public static void PostIncome(Income income)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                var old = context.Incomes.SingleOrDefault(i => i.Id == income.Id);

                if(old == null)
                {
                    income.AuthorId = user.UserId;
                    income.Number = context.Incomes.Where(i => i.DivisionId == income.DivisionId).Max(i => i.Number, 0) + 1;
                    income.Id = Guid.NewGuid();
                    if(income.IsFinAction)
                    {
                        income.DivisionId = null;
                    }
                    context.Incomes.AddObject(income);
                }
                else
                {
                    old.Amount = income.Amount;
                    old.CreatedOn = income.CreatedOn;
                    old.IncomeTypeId = income.IncomeTypeId;
                    old.PaymentType = income.PaymentType;
                    old.DivisionId = income.DivisionId;
                    old.Name = income.Name;
                    old.IsFinAction = income.IsFinAction;
                    if(old.IsFinAction)
                    {
                        old.DivisionId = null;
                    }
                }
                context.SaveChanges();
            }
        }

        public static List<Income> GetDivisionIncomes(Guid divisionId, DateTime start, DateTime end)
        {
            using(var context = new TonusEntities())
            {
                var cId = UserManagement.GetUser(context).CompanyId;
                start = start.Date;
                end = end.Date.AddDays(1);
                return context.Incomes.Where(i => (i.DivisionId == divisionId || (!i.DivisionId.HasValue && i.CompanyId == cId))
                    && i.CreatedOn >= start && i.CreatedOn < end).OrderByDescending(i => i.Number).ToList().Init();
            }
        }

        public static Guid PostTreatmentType(TreatmentType treatmentType)
        {
            using(var context = new TonusEntities())
            {
                if(context.TreatmentTypes.Any(i => i.Name.ToLower() == treatmentType.Name.ToLower() && i.Id != treatmentType.Id))
                {
                    throw new FaultException<string>("Тип услуг с таким названием уже создан!", "Тип услуг с таким названием уже создан!");
                }

                if(Thread.CurrentThread.CurrentCulture.Name != "ru-RU")
                {
                    var old = context.TreatmentTypes.FirstOrDefault(i => i.Id == treatmentType.Id);
                    if(old != null)
                    {
                        treatmentType.NameEn = treatmentType.Name;
                        treatmentType.Name = old.Name;
                    }
                }

                if(treatmentType.Id == Guid.Empty)
                {
                    treatmentType.NameEn = treatmentType.Name;
                }

                return Core.PostEntities("TreatmentTypes", new[] { treatmentType })[0];
            }
        }

        public static Guid PostTreatmentConfig(TreatmentConfig treatmentConfig)
        {
            using(var context = new TonusEntities())
            {
                if(context.TreatmentConfigs.Any(i => i.Name.ToLower() == treatmentConfig.Name.ToLower() && i.Id != treatmentConfig.Id))
                {
                    throw new FaultException<string>("Услуга с таким названием уже создана!", "Услуга с таким названием уже создана!");
                }

                if(Thread.CurrentThread.CurrentCulture.Name != "ru-RU")
                {
                    var old = context.TreatmentConfigs.FirstOrDefault(i => i.Id == treatmentConfig.Id);
                    if(old != null)
                    {
                        treatmentConfig.NameEn = treatmentConfig.Name;
                        treatmentConfig.Name = old.Name;
                    }
                }

                if(treatmentConfig.Id == Guid.Empty)
                {
                    treatmentConfig.NameEn = treatmentConfig.Name;
                }

                return Core.PostEntities("TreatmentConfigs", new[] { treatmentConfig })[0];
            }
        }

        public static void DeleteTreatmentProgram(Guid programId)
        {
            using(var context = new TonusEntities())
            {
                var pr = context.TreatmentPrograms.First(i => i.Id == programId);
                pr.IsAvail = false;
                context.SaveChanges();
            }
        }

        public static void PostLocalSettings(int keyDays, int keyPeriod, int licDays, int licPeriod, string notifyAddresses)
        {
            using(var context = new TonusEntities())
            {
                var ls = context.LocalSettings.FirstOrDefault();
                if(ls == null) return;
                ls.NotifyKeyDays = keyDays;
                ls.NotifyKeyPeriod = keyPeriod;
                ls.NotifyLicenseDays = licDays;
                ls.NotifyLicensePeriod = licPeriod;
                ls.NotifyAdresses = notifyAddresses;
                context.SaveChanges();
            }
        }

        public static LocalSetting GetLocalSetting()
        {
            using(var context = new TonusEntities())
            {
                var res = context.LocalSettings.FirstOrDefault();
                if(res != null)
                {
                    res.KeyValidTill = GetCertificateDate();
                }
                return res;
            }
        }

        public static X509Certificate2 GetCertificate()
        {
            X509Store my = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            my.Open(OpenFlags.ReadOnly);

            foreach(X509Certificate2 cert in my.Certificates)
            {
                if(cert.Thumbprint.Replace(" ", "").ToLower() == "d496ab9f8aa0b7ad4ff821158f90cce9037ef4af")
                {
                    return cert;
                }
            }
            throw new FaultException("Не обнаружен сертификат лицензирования!");

        }

        public static DateTime GetCertificateDate()
        {
            var fi = new FileInfo(ConfigurationManager.AppSettings.Get("CertPath"));
            if(!fi.Exists)
            {
                return DateTime.MinValue;
            }
            var len = (int)fi.Length;
            var res = new byte[len];
            var sr = new FileStream(ConfigurationManager.AppSettings.Get("CertPath"), FileMode.Open);
            sr.Read(res, 0, len);
            sr.Close();

            var hwkey = SyncCore.GetSystemId();

            var cert = GetCertificate();


            SHA1Managed sha1 = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            var cconf = CryptoConfig.MapNameToOID("SHA1");

            for(int i = 0; i < 730; i++)
            {
                var dat = DateTime.Today.AddDays(i);
                byte[] data = encoding.GetBytes(dat.ToString("yyyy-MM-dd") + " " + hwkey);

                byte[] hash = sha1.ComputeHash(data);

                if(VerifyString(hash, res, cert, cconf))
                {
                    return dat;
                }
            }
            return DateTime.MinValue;
        }

        public static bool VerifyString(byte[] hash, byte[] signature, X509Certificate2 cert, string cryptoconfig)
        {
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
            return csp.VerifyHash(hash, cryptoconfig, signature);
        }

        public static void PostSettingsFolder(SettingsFolder folder, Guid[] companies)
        {
            if(folder.ParentFolderId == Guid.Empty) folder.ParentFolderId = null;
            var companyId = Core.PostEntities("SettingsFolders", new[] { folder });
            using(var context = new TonusEntities())
            {
                var fld = context.SettingsFolders.Single(i => i.Id == folder.Id);
                fld.AccessingCompanies.Clear();
                foreach(var id in companies)
                {
                    fld.AccessingCompanies.Add(context.Companies.SingleOrDefault(i => i.CompanyId == id));
                }
                context.SaveChanges();

                context.TicketTypes.Where(i => i.SettingsFolderId == folder.Id).Select(i => i.Id).ToList().ForEach(i => Core.MaintainTicketTypeVisibility(i));
            }
        }

        public static void DeleteSettingsFolder(Guid folderId)
        {
            using(var context = new TonusEntities())
            {
                var fld = context.SettingsFolders.SingleOrDefault(i => i.Id == folderId);
                if(fld == null)
                {
                    DeleteCompanySettingsFolder(folderId);
                    return;
                }
                fld.AccessingCompanies.Clear();

                if(fld.CategoryId == 0)
                {
                    context.CustomerCardTypes.Where(i => i.SettingsFolderId == folderId).ToList().ForEach(i => i.SettingsFolderId = fld.ParentFolderId);
                }
                else if(fld.CategoryId == 1)
                {
                    context.TicketTypes.Where(i => i.SettingsFolderId == folderId).ToList().ForEach(i => i.SettingsFolderId = fld.ParentFolderId);
                }
                else if(fld.CategoryId == 2)
                {
                    context.Instalments.Where(i => i.SettingsFolderId == folderId).ToList().ForEach(i => i.SettingsFolderId = fld.ParentFolderId);
                }
                else if(fld.CategoryId == 3)
                {
                    context.TreatmentConfigs.Where(i => i.SettingsFolderId == folderId).ToList().ForEach(i => i.SettingsFolderId = fld.ParentFolderId);
                }

                else
                {
                    throw new Exception("Unsupported folder type!");
                }
                fld.SettingsFolders1.ToList().ForEach(i => i.ParentFolderId = fld.ParentFolderId);
                fld.SettingsFolders1.Clear();

                context.DeleteObject(fld);

                context.SaveChanges();
            }
        }

        public static void DeleteCompanySettingsFolder(Guid folderId)
        {
            using(var context = new TonusEntities())
            {
                var fld = context.CompanySettingsFolders.SingleOrDefault(i => i.Id == folderId);
                if(fld == null) return;

                if(fld.CategoryId == 0)
                {
                    context.TextActions.Where(i => i.SettingsFolderId == folderId).ToList().ForEach(i => i.SettingsFolderId = fld.ParentFolderId);
                }
                else if(fld.CategoryId == 1)
                {
                    context.TreatmentPrograms.Where(i => i.SettingsFolderId == folderId).ToList().ForEach(i => i.SettingsFolderId = fld.ParentFolderId);
                }
                else if(fld.CategoryId == 2)
                {
                    context.Corporates.Where(i => i.SettingsFolderId == folderId).ToList().ForEach(i => i.SettingsFolderId = fld.ParentFolderId);
                }
                else if(fld.CategoryId == 3)
                {
                    context.Treatments.Where(i => i.SettingsFolderId == folderId).ToList().ForEach(i => i.SettingsFolderId = fld.ParentFolderId);
                }
                else if(fld.CategoryId == 4)
                {
                    context.Solariums.Where(i => i.SettingsFolderId == folderId).ToList().ForEach(i => i.SettingsFolderId = fld.ParentFolderId);
                }
                else if(fld.CategoryId == 5)
                {
                    context.Storehouses.Where(i => i.SettingsFolderId == folderId).ToList().ForEach(i => i.SettingsFolderId = fld.ParentFolderId);
                }
                else if(fld.CategoryId == 6)
                {
                    context.Roles.Where(i => i.SettingsFolderId == folderId).ToList().ForEach(i => i.SettingsFolderId = fld.ParentFolderId);
                }
                else
                {
                    throw new Exception("Unsupported folder type!");
                }
                fld.CompanySettingsFolders1.ToList().ForEach(i => i.ParentFolderId = fld.ParentFolderId);
                fld.CompanySettingsFolders1.Clear();

                context.DeleteObject(fld);

                context.SaveChanges();
            }
        }

        public static void DeleteContraIndication(Guid contraId)
        {
            using(var context = new TonusEntities())
            {
                var contra = context.ContraIndications.SingleOrDefault(i => i.Id == contraId);
                contra.TreatmentTypes.Clear();
                contra.Customers.Clear();
                contra.IsVisible = false;

                context.SaveChanges();
            }
        }

        public static void PostCompanySettingsFolder(CompanySettingsFolder folder, Guid divId)
        {
            if(folder.CategoryId == 3 || folder.CategoryId == 4 || folder.CategoryId == 5) folder.DivisionId = divId;
            if(folder.ParentFolderId == Guid.Empty) folder.ParentFolderId = null;
            Core.PostEntities("CompanySettingsFolders", new[] { folder });
        }

        public static Company GetCompany()
        {
            using(var context = new TonusEntities())
            {
                return context.Users
                    .Where(u => u.UserName.ToLower() == OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name)
                    .Select(i => i.Company)
                    .Single();
            }
        }

        public static Guid GetCustomerIdByPhone(string phone)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var cust = context.Customers.FirstOrDefault(i => i.CompanyId == user.CompanyId && i.Phone2 == phone);
                if(cust == null) return Guid.Empty;
                return cust.Id;
            }
        }

        public static Dictionary<Guid, string> GetAllStatuses()
        {
            using(var context = new TonusEntities())
            {
                var cId = UserManagement.GetCompanyIdOrDefaultId(context);
                return context.CustomerStatuses.Where(i => i.IsAvail && i.CompanyId == cId).ToDictionary(i => i.Id, i => i.Name);
            }
        }

        public static Guid GetCustomerIdByTargetId(Guid targetId)
        {
            using(var context = new TonusEntities())
            {
                return context.CustomerTargets.Single(i => i.Id == targetId).CustomerId;
            }
        }

        public static Guid GetCustomerIdByTreatmentEventId(Guid treatmentEventId)
        {
            using(var context = new TonusEntities())
            {
                return context.TreatmentEvents.Single(i => i.Id == treatmentEventId).CustomerId;
            }
        }

        public static Guid GetCustomerIdByTicketId(Guid guid)
        {
            using(var context = new TonusEntities())
            {
                return context.Tickets.Single(i => i.Id == guid).CustomerId;
            }
        }

        public static Guid GetCustomerByCardId(Guid guid)
        {
            using(var context = new TonusEntities())
            {
                return context.CustomerCards.Single(i => i.Id == guid).CustomerId;
            }
        }

        public static Guid GetCustomerByGoodSale(Guid guid)
        {
            using(var context = new TonusEntities())
            {
                return context.GoodSales.Single(i => i.Id == guid).BarOrder.CustomerId;
            }
        }

        public static void PostMarkdown(Guid divisionId, Guid storeId, Guid goodId, string newName, decimal price, int amount, Guid provId)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var store = context.Storehouses.First(i => i.Id == storeId);
                var good = context.Goods.Single(i => i.Id == goodId);

                var prov = context.Providers.First(i => i.Id == provId);

                //Накладная на списание
                var cons = new Consignment
                {
                    DocType = 2,
                    AuthorId = user.UserId,
                    CompanyId = store.CompanyId,
                    Date = DateTime.Today,
                    DivisionId = divisionId,
                    Id = Guid.NewGuid(),
                    IsAsset = true,
                    Sdal = user.FullName,
                    SourceStorehouseId = storeId
                };
                cons.ConsignmentLines.Add(new ConsignmentLine
                {
                    AuthorId = user.UserId,
                    Comment = Localization.Resources.Markdown,
                    CompanyId = store.CompanyId,
                    ConsignmentId = cons.Id,
                    GoodId = goodId,
                    Id = Guid.NewGuid(),
                    Position = 1,
                    Quantity = amount
                });
                var year = DateTime.Today.Year;
                cons.Number = context.Consignments.Where(i => i.Date.Year == year && i.DocType == cons.DocType && i.CompanyId == store.CompanyId).Max(i => i.Number, 0) + 1;
                context.Consignments.AddObject(cons);

                //Новый товар
                var newGood = new Good
                {
                    AuthorId = user.UserId,
                    CompanyId = store.CompanyId,
                    CreatedOn = DateTime.Now,
                    Description = good.Description,
                    GoodsCategoryId = good.GoodsCategoryId,
                    Id = Guid.NewGuid(),
                    IntAmount = good.IntAmount,
                    IsVisible = true,
                    ManufacturerId = good.ManufacturerId,
                    ModifiedOn = null,
                    Name = newName,
                    ProductTypeId = good.ProductTypeId,
                    UnitTypeId = good.UnitTypeId
                };
                context.Goods.AddObject(newGood);

                //цена на новый товар
                var pr = new GoodPrice
                {
                    AuthorId = user.UserId,
                    CommonPrice = price,
                    CompanyId = store.CompanyId,
                    Date = DateTime.Now,
                    DivisionId = divisionId,
                    GoodId = newGood.Id,
                    Id = Guid.NewGuid(),
                    InPricelist = true
                };
                context.GoodPrices.AddObject(pr);

                //Приходная накладная
                var icons = new Consignment
                {
                    DocType = 0,
                    AuthorId = user.UserId,
                    CompanyId = store.CompanyId,
                    Date = DateTime.Today,
                    DivisionId = divisionId,
                    Id = Guid.NewGuid(),
                    IsAsset = true,
                    Sdal = user.FullName,
                    Prinal = user.FullName,
                    DestinationStorehouseId = storeId,
                    ProviderId = prov.Id
                };
                icons.Number = context.Consignments.Where(i => i.Date.Year == year && i.DocType == icons.DocType && i.CompanyId == store.CompanyId).Max(i => i.Number, 0) + 1;
                icons.ConsignmentLines.Add(new ConsignmentLine
                {
                    AuthorId = user.UserId,
                    Comment = Localization.Resources.Markdown,
                    CompanyId = store.CompanyId,
                    ConsignmentId = icons.Id,
                    GoodId = newGood.Id,
                    Id = Guid.NewGuid(),
                    Position = 1,
                    Quantity = amount
                });

                context.Consignments.AddObject(icons);

                context.SaveChanges();
            }
        }

        public static List<CustomerVisit> GetCustomerVisits(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                var res = context.CustomerVisits.Where(i => i.CustomerId == customerId).OrderByDescending(i => i.InTime).ToList();
                res.ForEach(i => i.Init());
                return res;
            }
        }

        public static List<string>[] GetWorkData()
        {
            using(var context = new TonusEntities())
            {

                var res = new List<string>[2];
                res[0] = (from c in context.Customers
                          select c.WorkPlace).Distinct().ToList();
                res[1] = (from c in context.Customers
                          select c.Position).Distinct().ToList();
                return res;
            }
        }

        public static void PostTicketTypeLimits(Guid ttId, KeyValuePair<Guid, int>[] lims)
        {
            using(var context = new TonusEntities())
            {
                var tt = context.TicketTypes.Single(i => i.Id == ttId);
                tt.TicketTypeLimits.ToList().ForEach(i => context.DeleteObject(i));

                foreach(var lim in lims)
                {
                    var l = new TicketTypeLimit
                    {
                        Amount = lim.Value,
                        Id = Guid.NewGuid(),
                        TicketTypeId = ttId,
                        TreatmentConfigId = lim.Key
                    };
                    context.TicketTypeLimits.AddObject(l);
                }

                context.SaveChanges();
            }
        }

        public static IEnumerable<Instalment> GetCompanyInstalments(bool activeOnly)
        {
            using(var context = new TonusEntities())
            {
                List<Instalment> res;

                var user = UserManagement.GetUser(context);
                if(activeOnly)
                {
                    res = user.Company.Instalments.Where(i => i.IsActive).ToList();
                }
                else
                {
                    res = context.Instalments.Where(i => i.IsActive).ToList().Where(i => i.SettingsFolderId == null || user.Company.AvailSettingsFolders.Any(j => j.Id == i.SettingsFolderId)).ToList();
                    res.ForEach(i =>
                    {
                        if(user.Company.Instalments.Any(j => j.Id == i.Id)) i.Helper = true;
                    });
                }
                //res.ForEach(i => i.Init());
                return res;
            }
        }

        public static void PostCompanyInstalmentEnable(Guid instalmentId, bool isEnabled)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                var instalment = context.Instalments.Single(i => i.Id == instalmentId);
                if(isEnabled)
                {
                    if(!user.Company.Instalments.Any(i => i.Id == instalmentId))
                    {
                        user.Company.Instalments.Add(instalment);
                    }
                }
                else
                {
                    if(user.Company.Instalments.Any(i => i.Id == instalmentId))
                    {
                        user.Company.Instalments.Remove(instalment);
                    }
                }

                context.SaveChanges();
            }
        }

        public static void SetActionDivisions(Guid res, Guid[] divisionIds)
        {
            using(var context = new TonusEntities())
            {
                var ta = context.TextActions.Single(i => i.Id == res);
                ta.Divisions.Clear();
                foreach(var id in divisionIds)
                {
                    var div = context.Divisions.Single(i => i.Id == id);
                    ta.Divisions.Add(div);
                }
                context.SaveChanges();
            }
        }
#if BEAUTINIKA
        public static void PostTicketCorrection(Guid guid, int NewLength, decimal NewUnits, decimal newExtra, decimal NewGuest, decimal NewSolarium, int NewFreeze, string Comment, DateTime? planInstDate)
#else
        public static void PostTicketCorrection(Guid guid, int NewLength, decimal NewUnits, decimal NewGuest, decimal NewSolarium, int NewFreeze, string Comment, DateTime? planInstDate, string newComment)
#endif
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var ticket = context.Tickets.Single(i => i.Id == guid);
                SetCorrection(context, ticket, user, "Length", NewLength, Comment);
                SetCorrection(context, ticket, user, "UnitsAmount", NewUnits, Comment);
                SetCorrection(context, ticket, user, "GuestUnitsAmount", NewGuest, Comment);
#if BEAUTINIKA
                SetCorrection(context, ticket, user, "ExtraUnitsAmount", newExtra, Comment);
#endif
                SetCorrection(context, ticket, user, "SolariumMinutes", NewSolarium, Comment);
                SetCorrection(context, ticket, user, "FreezesAmount", NewFreeze, Comment);
                SetCorrection(context, ticket, user, "Comment", newComment, Comment);
                SetCorrectionNullable(context, ticket, user, "PlanningInstalmentDay", planInstDate, Comment);
            }
        }
        private static void SetCorrection<T>(TonusEntities context, Ticket ticket, User user, string prop, T newValue, string comment)
        {
            var oldValue = (T)ticket.GetValue(prop);

            if(!object.Equals(oldValue, newValue))
            {
                ticket.SetValue(prop, newValue);

                var corr = new TicketCorrection
                {
                    Comment = comment,
                    CompanyId = ticket.CompanyId,
                    CreatedOn = DateTime.Now,
                    Id = Guid.NewGuid(),
                    NewValue = newValue.ToString(),
                    OldValue = oldValue == null ? "" : oldValue.ToString(),
                    PropertyName = GetPropertyName(prop),
                    TicketId = ticket.Id,
                    UserId = user.UserId
                };
                context.TicketCorrections.AddObject(corr);

                ticket.ModifiedBy = user.UserId;
                ticket.ModifiedOn = DateTime.Now;

                ticket.InitDetails();
                if(ticket.FinishDate.HasValue && ticket.FinishDate.Value > DateTime.Now) ticket.IsActive = true;

                context.SaveChanges();
            }
        }
        private static void SetCorrectionNullable(TonusEntities context, Ticket ticket, User user, string prop, DateTime? newValue, string comment)
        {
            var oldValue = (DateTime?)ticket.GetValue(prop);
            if(!oldValue.Equals(newValue))
            {
                ticket.SetValue(prop, newValue);

                var corr = new TicketCorrection
                {
                    Comment = comment,
                    CompanyId = ticket.CompanyId,
                    CreatedOn = DateTime.Now,
                    Id = Guid.NewGuid(),
                    NewValue = newValue.HasValue ? newValue.ToString() : "",
                    OldValue = oldValue.HasValue ? oldValue.ToString() : "",
                    PropertyName = GetPropertyName(prop),
                    TicketId = ticket.Id,
                    UserId = user.UserId
                };
                context.TicketCorrections.AddObject(corr);

                ticket.ModifiedBy = user.UserId;
                ticket.ModifiedOn = DateTime.Now;

                ticket.InitDetails();
                if(ticket.FinishDate.HasValue && ticket.FinishDate.Value > DateTime.Now) ticket.IsActive = true;

                context.SaveChanges();
            }
        }

        private static string GetPropertyName(string prop)
        {
            if(prop == "Length") return Localization.Resources.Duration;
            if(prop == "UnitsAmount") return Localization.Resources.TotalUnits;
            if(prop == "GuestUnitsAmount") return Localization.Resources.TotalGuest;
            if(prop == "SolariumMinutes") return Localization.Resources.TotalSol;
            if(prop == "FreezesAmount") return Localization.Resources.TotalFreeze;

            return prop;
        }

        public static string HideProviderOrder(Guid id)
        {
            using(var context = new TonusEntities())
            {
                var cons = context.Consignments.Single(i => i.Id == id);
                cons.Init();
                if(cons.ProviderPayments.Any() || cons.Consignments1.Any())
                {
                    return Localization.Resources.UnableToDeleteOrder;
                }
                cons.Comment = "#Deleted#";
                context.SaveChanges();
                return string.Empty;
            }
        }

        public static string GetMarkdownName(Guid goodId)
        {
            using(var context = new TonusEntities())
            {
                var good = context.Goods.Single(i => i.Id == goodId);
                if(!context.Goods.Any(i => i.Name == good.Name + Localization.Resources.mark))
                {
                    return good.Name + Localization.Resources.mark;
                }
                for(int x = 1; x < 100; x++)
                {
                    if(!context.Goods.Any(i => i.Name == good.Name + String.Format(Localization.Resources.marknum, x)))
                    {
                        return good.Name + String.Format(Localization.Resources.marknum, x);
                    }
                    x++;
                }
                return good.Name + Localization.Resources.mark;
            }
        }

        public static List<UnitCharge> GetCustomerUnitCharges(Guid customerId, DateTime start, DateTime end, bool addGuest)
        {
            using(var context = new TonusEntities())
            {
                return context.UnitCharges
                .Where(i => i.Ticket.CustomerId == customerId && i.Date >= start && i.Date < end && (addGuest || i.Charge > 0))
                .OrderBy(i => i.Date).ToList().Init();
            }
        }

        public static void UpdateVisitReceipt(Guid visitId, string receipt)
        {
            using(var context = new TonusEntities())
            {
                var vis = context.CustomerVisits.Single(i => i.Id == visitId);
                vis.Receipt = receipt;
                context.SaveChanges();
            }
        }

        public static bool IsFirstVisitEnabled(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                if(context.CustomerVisits.Any(i => i.CustomerId == customerId && i.OutTime.HasValue)) return false;
                if(context.Tickets.Any(i => i.CustomerId == customerId && (i.TicketType.IsGuest || i.TicketType.IsVisit || !i.TicketType.HasTestdrive))) return false;

                return true;
            }
        }

        public static Treatment GetTreatmentByMac(string macAddress)
        {
            using(var context = new TonusEntities())
            {
                var treatment = context.Treatments.FirstOrDefault(i => i.MacAddress.Contains(macAddress));
                return treatment;
            }
        }

        public static void SetTreatmentOnline(Guid treatmnetId, bool isOnline)
        {
            using(var context = new TonusEntities())
            {
                var treatment = context.Treatments.FirstOrDefault(i => i.Id == treatmnetId);
                if(treatment != null)
                {
                    treatment.IsOnline = isOnline;
                    context.SaveChanges();
                }
            }
        }

        public static IEnumerable<TreatmentConfig> GetAllTreatmentConfigs()
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                IEnumerable<TreatmentConfig> res = context.TreatmentConfigs.Where(i => i.IsActive).OrderBy(i => i.Name).ToList().Where(i => i.SettingsFolderId == null || user.Company.AvailSettingsFolders.Any(j => j.Id == i.SettingsFolderId));
                if(Thread.CurrentThread.CurrentCulture.Name != "ru-RU")
                {
                    res.ToList().ForEach(i => i.Name = i.NameEn);
                    res = res.OrderBy(i => i.Name).ToArray();
                }
                return res.ToList().Init();

            }
        }

        public static List<TreatmentType> GetAllTreatmentTypes()
        {
            using(var context = new TonusEntities())
            {
                var res = context.TreatmentTypes.ToList().Init();

                if(Thread.CurrentThread.CurrentCulture.Name != "ru-RU")
                {
                    res.ForEach(i => i.Name = i.NameEn);
                }

                return res.OrderBy(i => i.Name).ToList();
            }
        }

        public static List<ReportTemplate> GetAllTemplates()
        {
            using(var context = new TonusEntities())
            {
                var cid = UserManagement.GetCompanyIdOrDefaultId(context);
                var res = context.ReportTemplates.Where(i => i.CompanyId == cid).ToList();
                if(Thread.CurrentThread.CurrentCulture.Name != "ru-RU")
                {
                    res.ForEach(i =>
                    {
                        i.DisplayName = i.DisplayNameEn;
                        i.HtmlText = i.HtmlTextEn;
                    });
                }

                return res.OrderBy(i => i.DisplayName).ToList();
            }
        }

        public static void PostReportTemplate(ReportTemplate template)
        {
            using(var context = new TonusEntities())
            {
                var cid = UserManagement.GetCompanyIdOrDefaultId(context);
                var templ = context.ReportTemplates.First(i => i.Id == template.Id && i.CompanyId == cid);

                if(Thread.CurrentThread.CurrentCulture.Name != "ru-RU")
                {
                    templ.HtmlTextEn = template.HtmlText;
                }
                else
                {
                    templ.HtmlText = template.HtmlText;
                }

                templ.Description = template.Description;

                context.SaveChanges();
            }
        }

        public static void AddCommentToTreatmentEvent(Guid eventId, string comment)
        {
            using(var context = new TonusEntities())
            {
                var ev = context.TreatmentEvents.FirstOrDefault(i => i.Id == eventId);
                if(ev != null)
                {
                    ev.Comment = (comment ?? "").Trim();
                    context.SaveChanges();
                }
            }
        }

        public static byte[] GetCustomerImage(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                return context.Customers.Where(i => i.Id == customerId).Select(i => i.Image).FirstOrDefault();
            }
        }

        public static void UpdateCustomerImage(Guid customerId, byte[] imageBytes)
        {
            using(var context = new TonusEntities())
            {
                var cust = context.Customers.Single(i => i.Id == customerId);
                cust.Image = imageBytes;
                context.SaveChanges();
            }
        }

        public static void MoveTreatment(Guid treatmentId, bool isLeft)
        {
            using(var context = new TonusEntities())
            {
                var tre = context.Treatments.Single(i => i.Id == treatmentId);
                var tres = context.Treatments.Where(i => i.DivisionId == tre.DivisionId && i.IsActive).OrderBy(i => i.Order).ToList();
                tres.ForEach(i => i.Order = i.Order * 2);
                if(isLeft) tre.Order -= 3;
                else tre.Order += 3;
                int n = 1;
                tres.OrderBy(i => i.Order).ToList().ForEach(i => i.Order = n++);
                context.Treatments.Where(i => i.DivisionId == tre.DivisionId && !i.IsActive).OrderBy(i => i.Order).ToList().ForEach(i => i.Order = n++);
                context.SaveChanges();
            }
        }

        public static int GetTotalCustomerCharged(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                if(context.CustomerVisits.Where(i => i.CustomerId == customerId).Count() != 1) return -1;
                var ucs = context.UnitCharges.Where(i => i.Ticket.CustomerId == customerId);
                if(!ucs.Any()) return -1;
                return ucs.Sum(i => i.Charge);
            }
        }

        public static void PostTicketPayment(Guid ticketId, decimal pmtAmount)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var ticketPayment = new TicketPayment
                {
                    Amount = pmtAmount,
                    AuthorId = user.UserId,
                    CompanyId = user.CompanyId,
                    Id = Guid.NewGuid(),
                    PaymentDate = DateTime.Today,
                    TicketId = ticketId
                };
                context.TicketPayments.AddObject(ticketPayment);
                context.SaveChanges();
            }
        }

        public static List<News> GetNews()
        {
            using(var context = new TonusEntities())
            {
                var res = context.News.OrderByDescending(i => i.CreatedOn).ToList();
                var user = UserManagement.GetUser(context);

                if(user.Roles.Any(i => i.Permissions.Any(j => j.PermissionKey == "BeautyNature")))
                {
                    var descs = context.SshFiles.Where(i => i.Path.ToLower().Contains("/fm/vkurse/") && i.Path.ToLower().EndsWith("desc.txt"));
                    foreach(var desc in descs)
                    {
                        if(SshCore.DownloadFile(desc))
                        {
                            var streamReader = new StreamReader("c:\\temp\\" + desc.Id, Encoding.GetEncoding(1251));
                            string text = streamReader.ReadToEnd();
                            streamReader.Close();
                            string x;
                            if(user.Roles.Any(i => i.Permissions.Any(j => j.PermissionKey == "BeautyNature1")))
                            {
                                x = desc.Path.ToLower().Replace("desc.txt", "1.pdf");
                            }
                            else if(user.Roles.Any(i => i.Permissions.Any(j => j.PermissionKey == "BeautyNature2")))
                            {
                                x = desc.Path.ToLower().Replace("desc.txt", "2.pdf");
                            }
                            else if(user.Roles.Any(i => i.Permissions.Any(j => j.PermissionKey == "BeautyNature3")))
                            {
                                x = desc.Path.ToLower().Replace("desc.txt", "3.pdf");
                            }
                            else
                            {
                                x = desc.Path.ToLower().Replace("desc.txt", "4.pdf");
                            }
                            var file = context.SshFiles.SingleOrDefault(i => i.Path == x);
                            if(file != null)
                            {
                                var fId = file.Id;
                                var link = fId.ToString();
                                res.Add(new News
                                {
                                    CreatedOn = desc.ModifiedDate,
                                    Message = text,
                                    PrirodaId = desc.Id,
                                    UrlTitle = "Читать",
                                    Url = link,
                                    Subject = "Вышел новый номер газеты \"Будь в курсе\"!"
                                });
                            }
                        }
                    }
                }
                return res.OrderByDescending(i => i.CreatedOn).ToList();
            }
        }

        public static List<ServiceModel.Ssh.SshFolder> GetSshFolders()
        {
            using(var context = new TonusEntities())
            {
                var res = new Dictionary<string, SshFolder>();
                var fs = context.SshFiles.OrderBy(i => i.Filename).Select(i => i.Path).ToList();
                while(fs.Any())
                {
                    foreach(var f in fs.ToArray())
                    {
                        var curdir = TruncateLastPart(f);
                        var pardir = TruncateLastPart(curdir);

                        FixDirectoryTree(curdir, res);

                        //if (pardir == "/home/jers/data/biznes" || pardir=="/home/jers/data/startdisk")
                        //{
                        //    if (!res.ContainsKey(pardir))
                        //    {
                        //        res.Add(pardir, new SshFolder { Name = GetLastPart(curdir), Path = pardir, Children = new List<SshFolder>() });
                        //    }
                        //    fs.Remove(f);
                        //}
                        if(res.ContainsKey(pardir))
                        {
                            if(!res.ContainsKey(curdir))
                            {
                                var nf = new SshFolder { Name = GetLastPart(curdir).Replace("/", ""), Path = pardir, Children = new List<SshFolder>() };
                                res.Add(curdir, nf);
                                if(curdir != pardir)
                                {
                                    res[pardir].Children.Add(nf);
                                }
                            }
                            fs.Remove(f);
                        }
                    }
                }
                res.Values.ToList().ForEach(i => i.Children.Sort());
                var r = new List<SshFolder>();
                var user = UserManagement.GetUser(context);
#if BEAUTINIKA
                if(UserManagement.HasPermission(user, "SshBusiness") && res.ContainsKey("/home/sysadmin/fm/beautinika/business"))
                {
                    r.Add(res["/home/sysadmin/fm/beautinika/business"]);
                    res["/home/sysadmin/fm/beautinika/business"].Name = "Бизнес-навигатор";
                }
                if(UserManagement.HasPermission(user, "SshStart") && res.ContainsKey("/home/sysadmin/fm/beautinika/start"))
                {
                    r.Add(res["/home/sysadmin/fm/beautinika/start"]);
                    res["/home/sysadmin/fm/beautinika/start"].Name = "Старт-мастер";
                }
#else
                if(UserManagement.HasPermission(user, "SshBusiness"))
                {
                    r.Add(res["/business"]);
                    res["/business"].Name = "Бизнес-навигатор";

                    r.Add(new SshFolder { Name = "Бизнес-навигатор – последнее", Path = "~b/" });
                }
                if(UserManagement.HasPermission(user, "SshStart"))
                {
                    r.Add(res["/start"]);
                    res["/start"].Name = "Старт-мастер";

                    r.Add(new SshFolder { Name = "Старт-мастер – последнее", Path = "~s/" });
                }
                if(UserManagement.HasPermission(user, "SshCrysis"))
                {
                    r.Add(res["/crysismaster"]);
                    res["/crysismaster"].Name = "Кризис-мастер";

                    r.Add(new SshFolder { Name = "Кризис-мастер – последнее", Path = "~c/" });
                }
#endif
                //r.Add(res["/home/jers/data/priroda"]);
                //res["/home/jers/data/priroda"].Name = "Природа красоты";
                return r;
            }
        }

        private static void FixDirectoryTree(string curdir, Dictionary<string, SshFolder> res)
        {
            var parent = TruncateLastPart(curdir);
            if(parent != null && parent != "/")
            {
                FixDirectoryTree(parent, res);
            }
            if(!res.ContainsKey(parent))
            {
                var sf = new SshFolder { Name = GetLastPart(parent).Replace("/", ""), Children = new List<SshFolder>(), Path = TruncateLastPart(parent) };
                res.Add(parent, sf);
                if(GetLastPart(parent) != TruncateLastPart(parent))
                {
                    res[TruncateLastPart(parent)].Children.Add(sf);
                }
            }
        }

        private static string GetLastPart(string path)
        {
            var i = path.LastIndexOf("/");
            if(i >= 0) return path.Substring(i);
            return path;
        }

        private static string TruncateLastPart(string path)
        {
            var i = path.LastIndexOf("/");
            if(i > 0) return path.Substring(0, i);
            return "/";
        }

        public static List<SshFile> GetSshFiles()
        {
            using(var context = new TonusEntities())
            {
                var res = context.SshFiles.OrderBy(i => i.Filename).ToList();
                res.ForEach(i => i.LengthF = ((float)i.Length) / 1024);
                Logger.Log("GetSshFiles count:" + res.Count);

                var newFiles = res.Where(i => i.ModifiedDate > DateTime.Today.AddMonths(-3)).ToArray();
                foreach(var nf in newFiles)
                {
                    res.Add(new SshFile
                    {
                        Avail = true,
                        Filename = nf.Filename,
                        Id = Guid.NewGuid(),
                        Length = nf.Length,
                        LengthF = nf.LengthF,
                        Path = nf.Path,
                        ModifiedDate = nf.ModifiedDate
                    });
                }

                return res;
            }
        }

        public static void EnqueueSshFile(Guid fileId)
        {
            using(var context = new TonusEntities())
            {
                if(!context.SshFileTasks.Any(i => i.FileId == fileId))
                {
                    context.SshFileTasks.AddObject(new SshFileTask { Id = Guid.NewGuid(), FileId = fileId });
                    context.SaveChanges();
                }
            }
        }

        public static void MaintainTicketTypeVisibility(Guid ttId)
        {
            using(var context = new TonusEntities())
            {
                var tt = context.TicketTypes.Single(i => i.Id == ttId);

                if(tt.SettingsFolderId != null)
                {
                    var aVcomps = context.SettingsFolders.Single(i => i.Id == tt.SettingsFolderId).AccessingCompanies.Select(i => i.CompanyId).ToArray();
                    var comps = tt.Companies.Where(i => !aVcomps.Contains(i.CompanyId)).ToList();
                    comps.ForEach(i => tt.Companies.Remove(i));

                    context.SaveChanges();
                }
            }
        }

        public static void PostSpendingTypeRemove(Guid spendingTypeId)
        {
            using(var context = new TonusEntities())
            {
                var st = context.SpendingTypes.Single(i => i.Id == spendingTypeId);
                st.IsDeleted = true;
                context.SaveChanges();
            }
        }

        public static void PostIncomeTypeRemove(Guid incomeTypeId)
        {
            using(var context = new TonusEntities())
            {
                var it = context.IncomeTypes.Single(i => i.Id == incomeTypeId);
                it.IsDeleted = true;
                context.SaveChanges();
            }
        }

        public static void PostCustomerStatusDelete(Guid statusId)
        {
            using(var context = new TonusEntities())
            {
                var stat = context.CustomerStatuses.FirstOrDefault(i => i.Id == statusId);
                if(stat != null)
                {
                    stat.IsAvail = false;
                    stat.Customers.Clear();
                    context.SaveChanges();
                }
            }
        }

        public static void PostCustomerStatus(Guid statusId, string name)
        {
            using(var context = new TonusEntities())
            {
                var stat = context.CustomerStatuses.FirstOrDefault(i => i.Id == statusId);
                if(stat == null)
                {
                    stat = new CustomerStatus
                    {
                        Id = statusId,
                        CompanyId = UserManagement.GetCompanyIdOrDefaultId(context),
                        IsAvail = true
                    };
                    context.CustomerStatuses.AddObject(stat);
                }
                stat.Name = name;
                context.SaveChanges();
            }
        }

        public static IEnumerable<TreatmentConfig> GetAllTreatmentConfigsAdmin()
        {
            using(var context = new TonusEntities())
            {
                var res = context.TreatmentConfigs.ToList().Init();

                if(Thread.CurrentThread.CurrentCulture.Name != "ru-RU")
                {
                    res.ForEach(i => i.Name = i.NameEn);
                }

                return res.OrderBy(i => i.Name).ToArray();
            }
        }

        public static List<Ticket> GetTicketsForCustomer(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                var tickets = context.Tickets
                    .AddIncludes(new string[] { "Division", "TicketPayments", "TicketFreezes", 
                        "TicketFreezes.TicketFreezeReason", "Successors", "TicketType",
                        "MinutesCharges", "Division.Company", "SolariumVisits" })
                    .Where(i => i.CustomerId == customerId).ToList();

                var tArr = tickets.Select(i => i.Id).ToArray();

                var src = context.UnitCharges.Where(j => tArr.Contains(j.TicketId)).GroupBy(j => j.TicketId).Select(j => new
                {
                    Id = j.Key,
                    U = j.Sum(k => (int?)k.Charge) ?? 0,
                    G = j.Sum(k => (int?)k.GuestCharge) ?? 0,
                    A = j.Where(i => !i.EventId.HasValue).Sum(c => (int?)c.Charge) ?? 0

#if BEAUTINIKA
,
                    E = j.Sum(k => (int?)k.ExtraCharge) ?? 0
#endif
                }).ToArray();

                tickets.ForEach(i =>
                {
                    i.unitsOutInited = true;
                    var uc = src.Where(j => i.Id == j.Id).FirstOrDefault();
                    if(uc != null)
                    {
                        i.UnitsOut = uc.U;
                        i.GuestUnitsOut = uc.G;
                        i.UnitsOutAuto = uc.A;
#if BEAUTINIKA
                        i.ExtraUnitsOut = uc.E;
#endif
                    }
                    i.InitDetails();
                });
                return tickets;
            }
        }

        public static SshFile GetSshFile(Guid fileId)
        {
            return new TonusEntities().SshFiles.Single(i => i.Id == fileId);
        }

        public static void PostTicketTypeCustomerCardTypes(Guid ttId, Guid[] ccTypes)
        {
            using(var context = new TonusEntities())
            {
                var tt = context.GetObjectByKey(new EntityKey("TonusEntities.TicketTypes", "Id", ttId)) as TicketType;
                tt.CustomerCardTypes.Clear();
                foreach(var i in ccTypes)
                {
                    tt.CustomerCardTypes.Add(context.GetObjectByKey(new EntityKey("TonusEntities.CustomerCardTypes", "Id", i)) as CustomerCardType);
                }
                context.SaveChanges();
            }
        }

        public static List<CumulativeDiscount> GetCumulativeDiscounts(Guid companyId)
        {
            using(var context = new TonusEntities())
            {
                return context.CumulativeDiscounts
                    .Where(i => i.CompanyId == companyId).OrderBy(i => i.IsCountDisc)
                    .ThenBy(i => i.ValueFrom).ToList();
            }
        }


        public static CumulativeDiscountInfo GetCumulativeDiscountInfo(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                var res = new CumulativeDiscountInfo();
                var cId = UserManagement.GetCompanyIdOrDefaultId(context);
                var src = context.Tickets.Where(i => i.CustomerId == customerId && !i.TicketType.IsGuest && !i.TicketType.IsVisit)
                    .Where(i => !i.ReturnDate.HasValue)
                    .Where(i => !i.InheritedTicketId.HasValue)
                    .Select(i => new { i.Price, i.DiscountPercent }).ToArray();
                res.TicketsAmount = (src.Sum(i => (decimal?)i.Price * (1 - i.DiscountPercent)) ?? 0);
                res.TicketsCount = src.Count();
                var src2 = context.GoodSales
                    .Where(i => i.BarOrder.CustomerId == customerId && !i.ReturnById.HasValue)
                    .Select(i => new { i.Amount, Price = (i.PriceMoney ?? 0) }).ToArray();
                res.GoodsAmount = src2.Sum(i => (decimal?)i.Amount * i.Price) ?? 0;

                var cums = context.CumulativeDiscounts.Where(i => i.CompanyId == cId).ToArray();

                res.DiscountPercent = (cums.Where(i => (i.IsCountDisc && i.ValueFrom <= res.TicketsCount && i.ValueTo >= res.TicketsCount) ||
                    (!i.IsCountDisc && i.ValueFrom <= res.Amount && i.ValueTo >= res.Amount)).Max(i => (decimal?)i.DiscountPercent) ?? 0m);

                var nextRub = cums.Where(i => !i.IsCountDisc && i.DiscountPercent > res.DiscountPercent)
                    .Where(i => i.ValueFrom > res.Amount)
                    .OrderBy(i => i.ValueFrom).FirstOrDefault();
                var nextA = cums.Where(i => i.IsCountDisc && i.DiscountPercent > res.DiscountPercent)
                    .Where(i => i.ValueFrom > res.TicketsCount)
                    .OrderBy(i => i.ValueFrom).FirstOrDefault();

                if(nextRub != null)
                {
                    res.NextRub = nextRub.ValueFrom - res.Amount;
                    res.NextRubPercent = nextRub.DiscountPercent;
                }
                if(nextA != null)
                {
                    res.NextTickets = nextA.ValueFrom - res.TicketsCount;
                    res.NextTicketsPercent = nextA.DiscountPercent;
                }

                res.DiscountPercent = res.DiscountPercent / 100m;
                return res;
            }
        }

        public static List<CustomerEventView> GetCRMEvents(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                var res = new List<CustomerEventView>();
                res.AddRange(context.Calls.Where(i => i.CustomerId == customerId)
                    .Select(i => new CustomerEventView
                        {
                            Id = i.Id,
                            Date = i.StartAt,
                            TypeText = i.IsIncoming ? "Входящий звонок" : "Исходящий звонок",
                            Employee = i.CreatedBy.FullName,
                            Result = i.Result,
                            Comments = i.Log
                        }));
                res.AddRange(context.CustomerCrmEvents.Where(i => i.CustomerId == customerId)
                    .Select(i => new CustomerEventView
                    {
                        Id = i.Id,
                        Date = i.EventDate,
                        TypeText = i.Subject,
                        Employee = i.User.FullName,
                        Result = i.Result,
                        Comments = i.Comment
                    }));
                res.AddRange(context.CustomerNotifications.Where(i => i.CustomerId == customerId && !i.CompletedOn.HasValue)
                    .Select(i => new CustomerEventView
                    {
                        Id = i.Id,
                        Date = i.ExpiryDate,
                        TypeText = "[План] " + i.Subject,
                        Comments = i.Message,
                        IsCall = true
                    }));
                return res.OrderByDescending(i => i.Date).ToList();
            }
        }

        public static List<Package> GetPackages()
        {
            using(var context = new TonusEntities())
            {
                var cId = UserManagement.GetCompanyIdOrDefaultId(context);
                return context.Packages.Where(i => i.CompanyId == cId && i.IsActive).OrderBy(i => i.Name).ToList().Init();
            }
        }

        public static Guid PostPackage(Package package, IEnumerable<PackageLine> packageLines)
        {
            var res = Core.PostEntities("Packages", new[] { package })[0];
            using(var context = new TonusEntities())
            {
                var p = context.Packages.Single(i => i.Id == res);
                p.PackageLines.ToList().ForEach(i => context.DeleteObject(i));
                p.PackageLines.Clear();

                packageLines.ToList().ForEach(i => context.PackageLines.AddObject(new PackageLine
                {
                    Amount = i.Amount,
                    CompanyId = p.CompanyId,
                    GoodId = i.GoodId,
                    Id = Guid.NewGuid(),
                    PackageId = res
                }));
                context.SaveChanges();
                return res;
            }
        }

        public static List<GoodReserve> GetGoodsReserve(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                return context.CustomerGoodsFlows
                    .Where(i => i.CustomerId == customerId)
                    .GroupBy(i => i.Good)
                    .Select(i => new GoodReserve { GoodId = i.Key.Id, GoodName = i.Key.Name, Amount = (i.Sum(j => (int?)j.Amount)) ?? 0 })
                    .Where(i => i.Amount != 0)
                    .OrderBy(i => i.GoodName)
                    .ToList();
            }
        }

        public static bool GiveGoodToCustomer(Guid divisionId, Guid customerId, Guid goodId)
        {
            using(var context = new TonusEntities())
            {
                //var left = context.CustomerGoodsFlows
                //    .Where(i => i.CustomerId == customerId && i.GoodId == goodId && i.DivisionId == divisionId)
                //    .Sum(i => (int?)i.Amount) ?? 0;
                //if (left <= 0) return false;
                var user = UserManagement.GetUser(context);

                context.CustomerGoodsFlows.AddObject(new CustomerGoodsFlow
                {
                    Amount = -1,
                    CompanyId = user.CompanyId,
                    CreatedById = user.UserId,
                    CreatedOn = DateTime.Now,
                    CustomerId = customerId,
                    Description = "Выдача товара клиенту",
                    DivisionId = divisionId,
                    GoodId = goodId,
                    Id = Guid.NewGuid(),
                    StorehouseId = null
                });
                context.SaveChanges();
                return true;
            }
        }

        public static List<Division> GetDivisionsForCompany(Guid companyId)
        {
            using(var context = new TonusEntities())
            {
                return context.Divisions.Where(i => i.CompanyId == companyId).OrderBy(i => i.Name).ToList();
            }
        }
#if!BEAUTINIKA
        public static List<CustomerTargetType> GetTargetTypes()
        {
            using(var context = new TonusEntities())
            {
                return context.CustomerTargetTypes.Where(i => i.IsAvail).OrderBy(i => i.Name).ToList().Init();
            }
        }

        public static void SetTargetTypeRecs(Guid id, Guid[] recomendations)
        {
            using(var context = new TonusEntities())
            {
                var tt = context.CustomerTargetTypes.Single(i => i.Id == id);
                tt.TreatmentConfigs.Clear();

                var tts = context.TreatmentConfigs.Where(i => recomendations.Contains(i.Id)).ToArray();

                foreach(var t in tts)
                {
                    tt.TreatmentConfigs.Add(t);
                }

                context.SaveChanges();
            }
        }
#endif

        public static void HideTargetTypeById(Guid targetTypeId)
        {
            if(targetTypeId == Guid.Empty) return;
            using(var context = new TonusEntities())
            {
                var tt = context.CustomerTargetTypes.Single(i => i.Id == targetTypeId);
                tt.IsAvail = false;
                context.SaveChanges();
            }
        }

        public static List<BarDiscount> GetBarDiscounts()
        {
            using(var context = new TonusEntities())
            {
                var companyId = UserManagement.GetCompanyIdOrDefaultId(context);
                return context.BarDiscounts
                    .Where(i => i.CompanyId == companyId).OrderBy(i => i.ValueFrom).ToList();
            }
        }

        public static decimal GetBarDiscountForCustomer(Guid customerId)
        {
            using(var context = new TonusEntities())
            {
                var d = context.DepositAccounts.Where(i => i.CustomerId == customerId && i.Amount > 0)
                    .OrderByDescending(i => i.CreatedOn)
                    .FirstOrDefault();
                if(d == null) return 0;
                return
                    context.BarDiscounts.Where(
                        i => i.CompanyId == d.CompanyId && i.ValueFrom <= d.Amount && i.ValueTo >= d.Amount)
                        .Max(i => (decimal?)i.DiscountPercent) ?? 0;
            }
        }
#if!BEAUTINIKA
        public static List<TargetTypeSet> GetTargetConfigs()
        {
            using(var context = new TonusEntities())
            {
                var res = context.TargetTypeSets
                    .Include("CustomerTargetType")
                    .Include("CustomerTargetType")
                    .Where(i => i.CustomerTargetType.IsAvail).OrderBy(i => i.CustomerTargetType.Name).ToList().Init();
                var cs = context.TreatmentConfigs.ToList();
                res.ForEach(i => i.InitConfigs(cs));
                return res;
            }
        }
#endif

        public static void PostBonusCorrection(Guid customerId, int amount)
        {
            using(var context = new TonusEntities())
            {
                var str = amount > 0 ? "Начисление вручную" : "Списание вручную";
                var user = UserManagement.GetUser(context);
                context.BonusAccounts.AddObject(new BonusAccount
                {
                    Amount = amount,
                    AuthorId = user.UserId,
                    CompanyId = user.CompanyId,
                    CustomerId = customerId,
                    Description = str,
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.Now
                });
                context.SaveChanges();
            }
        }

        public static void PostCustomerInvitor(Guid customerId, Guid? invitorId)
        {
            using(var context = new TonusEntities())
            {
                if(customerId == invitorId) return;
                var cust = context.Customers.Single(i => i.Id == customerId);
                var invName = invitorId.HasValue ? context.Customers.Where(i => i.Id == invitorId).Select(i => i.LastName).FirstOrDefault() : "Не указан";
                Logger.DBLog("Изменение пригласившего клиента для {0} на {1}", cust.FullName, invName);

                cust.InvitorId = invitorId;
                context.SaveChanges();
            }
        }

        public static Guid PostMeasure(CustomerMeasure measure)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                var orig = context.CustomerMeasures.FirstOrDefault(i => i.Id == measure.Id);

                if(orig == null && measure.Deleted) return Guid.Empty;

                if((orig == null || measure.Id == Guid.Empty) && !measure.Deleted)
                {
                    if(measure.Id == Guid.Empty)
                    {
                        measure.Id = Guid.NewGuid();
                    }
                    measure.AuthorId = user.UserId;
                    measure.CreatedOn = DateTime.Now;
                    measure.CompanyId = user.CompanyId;

                    context.CustomerMeasures.AddObject(measure);
                    var customer = context.Customers.First(i => i.Id == measure.CustomerId);
                    context.Tasks.AddObject(new Task
                    {
                        AuthorId = user.UserId,
                        CompanyId = user.CompanyId,
                        CreatedOn = DateTime.Now.AddDays(21),
                        ExpiryOn = DateTime.Now.AddMonths(1).AddDays(1),
                        Id = Guid.NewGuid(),
                        Message = String.Format("Выполнение замеров по клиенту {0} {1} {2}. Месяц назад были произведены замеры, необходимо провести повторные.", customer.LastName, customer.FirstName, customer.MiddleName),
                        Priority = 2,
                        Subject = String.Format("Выполнение замеров по клиенту {0} {1} {2}", customer.LastName, customer.FirstName, customer.MiddleName),
                        StatusId = 0
                    });
                }
                else
                {
                    if(measure.Deleted)
                    {
                        if(orig != null) context.ObjectStateManager.ChangeObjectState(orig, EntityState.Deleted);
                    }
                    else
                    {
                        context.Detach(orig);
                        context.CustomerMeasures.Attach(measure);
                        context.ObjectStateManager.ChangeObjectState(measure, EntityState.Modified);
                    }
                }

                context.SaveChanges();
                return measure.Id;
            }
        }

        public static Guid PostCustomerAnthropomentric(Anthropometric anthropometric)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                var orig = context.Anthropometrics.FirstOrDefault(i => i.Id == anthropometric.Id);

                if(orig == null && anthropometric.Deleted) return Guid.Empty;

                if((orig == null || anthropometric.Id == Guid.Empty) && !anthropometric.Deleted)
                {
                    if(anthropometric.Id == Guid.Empty)
                    {
                        anthropometric.Id = Guid.NewGuid();
                    }
                    anthropometric.AuthorId = user.UserId;
                    anthropometric.CreatedOn = DateTime.Now;
                    anthropometric.CompanyId = user.CompanyId;

                    context.Anthropometrics.AddObject(anthropometric);
                    var customer = context.Customers.First(i => i.Id == anthropometric.CustomerId);
                    context.Tasks.AddObject(new Task
                    {
                        AuthorId = user.UserId,
                        CompanyId = user.CompanyId,
                        CreatedOn = DateTime.Now.AddDays(21),
                        ExpiryOn = DateTime.Now.AddMonths(1).AddDays(1),
                        Id = Guid.NewGuid(),
                        Message = String.Format("Выполнение замеров по клиенту {0} {1} {2}. Месяц назад были произведены замеры, необходимо провести повторные.", customer.LastName, customer.FirstName, customer.MiddleName),
                        Priority = 2,
                        Subject = String.Format("Выполнение замеров по клиенту {0} {1} {2}", customer.LastName, customer.FirstName, customer.MiddleName),
                        StatusId = 0
                    });
                }
                else
                {
                    if(anthropometric.Deleted)
                    {
                        if(orig != null) context.ObjectStateManager.ChangeObjectState(orig, EntityState.Deleted);
                    }
                    else
                    {
                        context.Detach(orig);
                        context.Anthropometrics.Attach(anthropometric);
                        context.ObjectStateManager.ChangeObjectState(anthropometric, EntityState.Modified);
                    }
                }

                context.SaveChanges();
                return anthropometric.Id;
            }
        }

        public static void PostBonusCorrection(Guid customerId, int amount, string comment)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                context.BonusAccounts.AddObject(new BonusAccount
                {
                    Amount = amount,
                    AuthorId = user.UserId,
                    CompanyId = user.CompanyId,
                    CustomerId = customerId,
                    Description = comment,
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.Now
                });
                context.SaveChanges();
            }
        }

        public static bool PostExtraSmart(Guid customerId, string comment)
        {
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var ticket = context.Tickets.OrderByDescending(i => i.CreatedOn).FirstOrDefault(i => i.TicketType.IsSmart && i.CustomerId == customerId && i.IsActive && !i.ReturnDate.HasValue && !i.Successors.Any());
                if(ticket == null)
                {
                    return false;
                }
                SetCorrection(context, ticket, user, "UnitsAmount", ticket.UnitsAmount + 8, comment);
                context.SaveChanges();

                return true;
            }
        }

        public static CustomerNotification GetCustomerNotificationById(Guid notificationId)
        {
            using(var context = new TonusEntities())
            {
                return context.CustomerNotifications.Single(i => i.Id == notificationId);
            }
        }

        public static string GetFromSite(Guid divisionId, DateTime fromTime)
        {
            using(var context = new TonusEntities())
            {
                var source = context.Customers.Where(i => i.ClubId == divisionId && i.FromSite && i.CreatedOn >= fromTime).Select(i => new
                {
                    i.LastName,
                    i.FirstName,
                    i.MiddleName,
                    i.Phone2
                }).ToList();
                if(!source.Any())
                {
                    return null;
                }

                var sb = new StringBuilder();

                sb.AppendLine("Новые контакты с сайта www.tonusclub.ru:");

                foreach(var cust in source)
                {
                    sb.AppendFormat("{0} {1} {2}, тел.: {3}\n", cust.LastName, cust.FirstName, cust.MiddleName, cust.Phone2);
                }

                return sb.ToString();
            }
        }
    }

    public class IdName
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
