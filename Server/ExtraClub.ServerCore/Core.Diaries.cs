using System;
using System.Collections.Generic;
using System.Linq;
using ExtraClub.Entities;
using ExtraClub.ServiceModel;

namespace ExtraClub.ServerCore
{
    partial class Core
    {
        public static void ClearCustomerContras(Guid customerId)
        {
            using (var context = new ExtraEntities())
            {
                var customer = context.Customers.Single(c => c.Id == customerId);
                customer.NoContraIndications = true;
                customer.ContraIndications.Clear();

                Logger.DBLog("Удаление противопоказаний клиента. {0}", customer.FullName);

                context.SaveChanges();
            }
        }

        public static List<Guid> GetCustomerContrasIds(Guid customerId)
        {
            using (var context = new ExtraEntities())
            {
                var customer = context.Customers.Single(c => c.Id == customerId);
                var res = new List<Guid>();

                customer.ContraIndications.ToList().ForEach(i => res.Add(i.Id));

                return res;
            }
        }

        public static void PostContraIndications(Guid customerId, List<Guid> contraIds)
        {
            using (var context = new ExtraEntities())
            {
                var customer = context.Customers.Single(c => c.Id == customerId);

                customer.ContraIndications.Clear();
                customer.NoContraIndications = contraIds.Count == 0;

                foreach (var id in contraIds)
                {
                    var contra = context.ContraIndications.Single(c => c.Id == id);
                    customer.ContraIndications.Add(contra);
                }
                Logger.DBLog("Сохранение противопоказаний клиента ({0} шт). {1}", contraIds.Count, customer.FullName);

                context.SaveChanges();
            }
        }


        public static void PostContraIndicationTreatmentTypes(Guid contraId, List<Guid> treatmentIds)
        {
            using (var context = new ExtraEntities())
            {
                var contra = context.ContraIndications.Single(i => i.Id == contraId);
                contra.TreatmentTypes.Clear();

                foreach (var tId in treatmentIds)
                {
                    var treatment = context.TreatmentTypes.Single(i => i.Id == tId);
                    contra.TreatmentTypes.Add(treatment);
                }

                context.SaveChanges();
            }
        }


        public static List<CustomerTarget> GetCustomerTargets(Guid customerId)
        {
            using (var context = new ExtraEntities())
            {
                var customer = context.Customers.Single(c => c.Id == customerId);
                return customer.CustomerTargets.ToList().Init();
            }
        }


        public static List<Anthropometric> GetAnthropometricsForCustomer(Guid customerId)
        {
            using (var context = new ExtraEntities())
            {
                var res = context.Anthropometrics.Where(a => a.CustomerId == customerId);
                return res.ToList();
            }
        }


        public static List<DoctorVisit> GetDoctorVisitsForCustomer(Guid customerId)
        {
            using (var context = new ExtraEntities())
            {
                var res = context.DoctorVisits.Where(a => a.CustomerId == customerId);
                return res.ToList();
            }
        }

        public static List<string> GetGetDoctorTemplates()
        {
            using (var context = new ExtraEntities())
            {
                return (from i in context.DoctorVisits
                        select i.Doctor).Distinct().ToList();
            }
        }

        public static List<Nutrition> GetNutritionsForCustomer(Guid customerId)
        {
            using (var context = new ExtraEntities())
            {
                var res = context.Nutritions.Where(a => a.CustomerId == customerId);
                return res.ToList();
            }
        }

        public static List<string>[] GetNutritionTemplates()
        {
            using (var context = new ExtraEntities())
            {
                var res = new List<List<string>>();
                res.Add((from i in context.Nutritions
                         select i.Diet).Distinct().ToList());
                res.Add((from i in context.Nutritions
                         select i.Product).Distinct().ToList());

                return res.ToArray();
            }
        }

        public static List<CustomerMeasure> GetMeasuresForCustomer(Guid customerId)
        {
            using (var context = new ExtraEntities())
            {
                var res = context.CustomerMeasures.Where(a => a.CustomerId == customerId);
                return res.ToList();
            }
        }

        public static IEnumerable<Tuple<string, List<string>>> GetTreatmentTypesForCustomerGoals(Guid divisionId, Guid customerId)
        {
            using (var context = new ExtraEntities())
            {
                var goals = context.CustomerTargets
                    .Where(i => i.CustomerId == customerId && !(i.TargetComplete ?? false))
                    .Select(i => new
                    {
                        i.CustomerTargetType.Name,
                        i.TargetText,
                        ConfigSets = i.CustomerTargetType.TargetTypeSets.Select(j => j.TreatmentConfigIds)
                    }).AsEnumerable()
                    .SelectMany(i => i.ConfigSets.Select(j => new
                    {
                        i.Name,
                        i.TargetText,
                        TreatmentConfigIds = j.Split(',').Select(k => Guid.Parse(k))
                    }));

                var available = new HashSet<Guid>(context.Treatments
                    .Where(i => i.DivisionId == divisionId)
                    .SelectMany(i => i.TreatmentType.TreatmentConfigs.Select(j => j.Id))
                    .Distinct()
                    .AsEnumerable());

                var tcNames = context.TreatmentConfigs.ToDictionary(i => i.Id, i => i.Name);

                var getAdditionalText = new Func<string, string>(i =>
                {
                    return String.IsNullOrWhiteSpace(i) ? "" : " - " + i;
                });

                var contras = new HashSet<Guid>(context.Customers.Where(i => i.Id == customerId)
                    .SelectMany(i => i.ContraIndications.SelectMany(j => j.TreatmentTypes).SelectMany(j => j.TreatmentConfigs))
                    .Select(i => i.Id).AsEnumerable());

                return goals
                    .Where(i => i.TreatmentConfigIds.All(j => available.Contains(j)))
                    .Where(i => i.TreatmentConfigIds.All(j => !contras.Contains(j)))
                    .Select(i => Tuple.Create($"{i.Name}{getAdditionalText(i.TargetText)}",
                    i.TreatmentConfigIds.Select(j => tcNames[j]).ToList())).ToList();
            }
        }
    }
}
