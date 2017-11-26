using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.ServiceModel;
using ExtraClub.ServiceModel;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.EntityClient;
using System.Data.SqlClient;
using ExtraClub.ServiceModel.Sync;
using System.Collections.Concurrent;
using System.Threading;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Data;
using ExtraClub.Entities;

using Microsoft.Win32;
using System.Configuration;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace ExtraClub.ServerCore
{
    #region orphography
    public static class SyncCore
    {
        static Dictionary<string, string> KeyExcepts = new Dictionary<string, string>();
        static Dictionary<string, string> EntSetExcepts = new Dictionary<string, string>();
        static SyncCore()
        {
            KeyExcepts.Add("ManufacturerId", "Id");
            KeyExcepts.Add("GoodId", "Id");
            //KeyExcepts.Add("PermissionId", "Id");
            //KeyExcepts.Add("RoleId", "Id");
            EntSetExcepts.Add("Log", "Logs");
            EntSetExcepts.Add("TreatmentConfig", "TreatmentConfigs");
            EntSetExcepts.Add("ChildrenRoom", "ChildrenRooms");
            EntSetExcepts.Add("DictionaryInfo", "DictionaryInfos");
            EntSetExcepts.Add("SalaryScheme", "SalarySchemes");
            EntSetExcepts.Add("ContraIndicationsTreatmentTypes", "ContraIndicationsTreatmentTypes1");
            EntSetExcepts.Add("ContraIndicationsUsers", "ContraIndicationsUsers1");
        }

        static string FixKey(string key)
        {
            if(KeyExcepts.ContainsKey(key)) return KeyExcepts[key];
            return key;
        }

        static string FixEntSet(string key)
        {
            if(EntSetExcepts.ContainsKey(key)) return EntSetExcepts[key];
            return key;
        }

        static string FixEntSetBack(string value)
        {
            if(EntSetExcepts.ContainsValue(value)) return EntSetExcepts.Keys.First(i => EntSetExcepts[i] == value);
            return value;
        }
    #endregion

        static DateTime LastCustomerSync = DateTime.Today;

        public static void Syncronize()
        {
            return;
            //var user = UserManagement.GetUser(context);
            if(new ExtraEntities().Companies.Count() > 1)
            {
                throw new Exception("Синхронизация недопустима!");
            }

            ChannelFactory<ISyncService> cf = new ChannelFactory<ISyncService>("SyncServiceEndpoint");
            var client = cf.CreateChannel();
            var log = "";
            try
            {
                var sysId = GetSystemId();
                var stream = client.GetServerPart(sysId, GetLocalVersion());

                bool isSecond;
                Stream str = null;

                using(var context = new ExtraEntities())
                {
                    isSecond = context.Companies.Any();


                    if(isSecond)
                    {
                        var company = context.Companies.Single();
                        str = GetDataSetStream(company.CompanyId, GetVersion(), (id, ver1) => GetLocalDelta(context, id, ver1), ref log, -company.UtcCorr);
                    }
                    log += "Начинаем применение серверной части...\n";
                    CommitStream(stream, ref log);
                    log += "Применили серверную часть.\n";
                }
                using(var context = new ExtraEntities())
                {
                    var company = context.Companies.Single();
                    log += "Получаем номер локальной версии...\n";

                    var localver = GetCurrentChangeTrackingVersion(context);
                    log += "Получаем поток для локальной дельты...\n";

                    if(!isSecond)
                    {
                        str = GetDataSetStream(company.CompanyId, localver, (id, ver1) => GetLocalDelta(context, id, ver1), ref log, -company.UtcCorr);
                    }
                    Logger.Log("Размер клиентской дельты: " + str.Length);


                    //FileStream file = new FileStream("c:\\temp\\client\\" + DateTime.Now.ToString().Replace(":", "_") + ".bin", FileMode.Create, System.IO.FileAccess.Write);
                    //byte[] bytes = new byte[str.Length];
                    //str.Read(bytes, 0, (int)str.Length);
                    //file.Write(bytes, 0, bytes.Length);
                    //file.Close();
                    //str.Position = 0;

                    log += "Отправляем локальную дельту...\n";

                    var newVer = client.PostClientPart(str);
                    log += "Отправляем информацию об успешной синхронизации...\n";

                    {
                        client.PostSyncSuccess(GetSystemId(), newVer);
                    }
                    log += "Устанавливаем локальную версию...\n";

                    SetVersion(localver);
                    context.Claims.Where(i => i.StatusDescription == "Ожидает синхронизации").ToList().ForEach(i =>
                    {
                        i.StatusDescription = "Ожидает синхронизации с FTM";
                        i.StatusId = 1;
                    });
                    context.SaveChanges();
                }
                ////Загрузка природы красоты
                //try
                //{
                //    using (var context = new ExtraEntities())
                //    {
                //        var prir = context.SshFiles.Where(i => i.Path.Contains("/priroda/")).Select(i => i.Id).ToList();
                //        prir.ForEach(i =>
                //        {
                //            var f = new FileInfo(Path.Combine("c:\\temp\\", i.ToString()));
                //            if (!f.Exists && !context.SshFileTasks.Any(x => x.FileId == i))
                //            {
                //                context.SshFileTasks.AddObject(new SshFileTask
                //                {
                //                    Id = Guid.NewGuid(),
                //                    FileId = i
                //                });
                //            }
                //        });
                //        context.SaveChanges();
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Logger.Log(ex);
                //}
            }
            catch(EndpointNotFoundException ex)
            {
                log += "ОШИБКА: Невозможно подключиться к серверу синхронизации. Адрес, по которому была проведена попытка подключения: " + cf.Endpoint.Address.Uri.ToString();
                throw ex;
            }
            catch(Exception ex)
            {
                var ex1 = ex;
                while(ex1 != null)
                {
                    log += ex1.GetType().ToString() + "\n" + ex1.Message + "\n" + ex1.StackTrace;
                    ex1 = ex1.InnerException;
                }
                throw ex;
            }
            finally
            {
                ReportLog(log);
            }
            try
            {
                UpdateLicenseKey();
                UpdateLicenseExpiry(client);
            }
            catch(Exception _ex)
            {
                var _ex1 = _ex;
                while(_ex1 != null)
                {
                    log += _ex1.GetType().ToString() + "\n" + _ex1.Message + "\n" + _ex1.StackTrace;
                    _ex1 = _ex1.InnerException;
                }
            }
        }

        private static int GetLocalVersion()
        {
            using(var context = new ExtraEntities())
            {
                if(!context.LocalSettings.Any()) throw new Exception("Ошибочный контекст синхронизации!");
                return context.LocalSettings.Single().DbVersion;
            }
        }

        public static void UpdateLicenseKey()
        {
            return;
            if(new ExtraEntities().Companies.Count() > 1)
            {
                throw new Exception("Операция недопустима!");
            }

            ChannelFactory<ISyncService> cf = new ChannelFactory<ISyncService>("SyncServiceEndpoint");
            var client = cf.CreateChannel();
            var licKey = client.GetLicenceKey(GetSystemId());

            var sr = new FileStream(ConfigurationManager.AppSettings.Get("CertPath"), FileMode.Create);
            sr.Write(licKey, 0, licKey.Length);
            sr.Close();
        }

        private static void UpdateLicenseExpiry(ISyncService client)
        {
            return;
            using(var context = new ExtraEntities())
            {
                var ls = context.LocalSettings.First();
                var sysId = GetSystemId();
                ls.LicenseExpiry = client.GetExpiryDate(sysId);
                if(!ls.DefaultDivisionId.HasValue)
                {
                    ls.DefaultDivisionId = client.GetMainDivision(sysId);
                }
                context.SaveChanges();
            }
        }

        public static void ReportLog(string log)
        {
            var path = ConfigurationManager.AppSettings.Get("LogPath");
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            NotificationCore.SendLocalMail(log);
            var sw = new StreamWriter(path + "\\" + DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + ".log", true);
            sw.Write(log.Replace("\n", "\r\n"));
            sw.Close();
        }

        private static DataSet GetLocalDelta(ExtraEntities context, Guid companyId, long version)
        {
            using(var conn = new SqlConnection(((EntityConnection)context.Connection).StoreConnection.ConnectionString))
            {

                conn.Open();
                var cmd = new SqlCommand("sync_GetChanges", conn) { CommandType = System.Data.CommandType.StoredProcedure };
                cmd.CommandTimeout = 600;
                cmd.Parameters.AddWithValue("company", companyId);
                cmd.Parameters.AddWithValue("version", version);
                var da = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                da.Fill(ds);

                return ds;
            }
        }

        public static void GetDataSetStreamEx(Guid companyId, long version, Func<Guid, long, DataSet> func, ref string log, int utcCorr, string path)
        {
            try
            {
                FileStream s = new FileStream(Path.Combine(path, "stream.sync"), FileMode.Create, System.IO.FileAccess.Write);
                using(var master = new ExtraClub.Entities.ExtraEntities())
                using(var masterHelper = new ExtraClub.Entities.ExtraEntities())
                {
                    log += "Установлено подключение к БД\n";
                    master.ContextOptions.ProxyCreationEnabled = false;
                    var container = master.MetadataWorkspace.GetEntityContainer(master.DefaultContainerName, System.Data.Metadata.Edm.DataSpace.CSpace);
                    log += "Получен контейнер метаданных сущностей\n";
                    var sw = Stopwatch.StartNew();
                    var keys = func(companyId, version);
                    sw.Stop();
                    Debug.WriteLine("get sync keys take " + sw.ElapsedMilliseconds);
                    log += "Получены ключи обектов, входящих в текущую дельту\n";
                    var vers = GetCurrentChangeTrackingVersion(master);
                    var ver = System.BitConverter.GetBytes(vers);
                    s.Write(ver, 0, ver.Length);
                    log += "Текущая версия БД: " + vers + "\n";
                    var src = new BlockingCollection<KeyValuePair<byte, object>>();

                    log += "Запуск процесса поиска сущностей текущей дельты\n";

                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        ProcessEntitySearch(keys, container, master, src, masterHelper, companyId);
                    });

                    log += "Запуск процесса записи потока сущностей текущей дельты\n";
                    log += "UTCCORR: " + utcCorr + "\n";
                    int n = 0;
                    foreach(var item in src.GetConsumingEnumerable())
                    {
                        n++;
                        CorrectItemUtc(item.Value, utcCorr, ref log);
                        BinaryFormatter ser = new BinaryFormatter();
                        var ms = new MemoryStream();
                        var zs = new GZipStream(ms, CompressionMode.Compress);
                        ser.Serialize(zs, item);
                        zs.Close();
                        var arr = ms.ToArray();
                        var h = System.BitConverter.GetBytes(arr.Length);
                        s.Write(h, 0, h.Length);
                        s.Write(arr, 0, arr.Length);
                    }
                    Debug.WriteLine("Total objects: " + n);
                    log += "Процесс записи потока сущностей текущей дельты завершен\n";
                }
                Debug.WriteLine("Total stream length: " + s.Length);
                s.Close();
            }
            catch(Exception ex)
            {
                log += "Сбой при получении потока данных дельты.\n" + ex.Message + "\n" + ex.StackTrace + "\n";
                throw ex;
            }
        }

        public static Stream GetDataSetStream(Guid companyId, long version, Func<Guid, long, DataSet> func, ref string log, int utcCorr)
        {
            try
            {
                var s = new MemoryStream();
                using(var master = new ExtraClub.Entities.ExtraEntities())
                using(var masterHelper = new ExtraClub.Entities.ExtraEntities())
                {
                    log += "Установлено подключение к БД\n";
                    master.ContextOptions.ProxyCreationEnabled = false;
                    var container = master.MetadataWorkspace.GetEntityContainer(master.DefaultContainerName, System.Data.Metadata.Edm.DataSpace.CSpace);
                    log += "Получен контейнер метаданных сущностей\n";
                    var sw = Stopwatch.StartNew();
                    var keys = func(companyId, version);
                    sw.Stop();
                    Debug.WriteLine("get sync keys take " + sw.ElapsedMilliseconds);
                    log += "Получены ключи обектов, входящих в текущую дельту\n";
                    var vers = GetCurrentChangeTrackingVersion(master);
                    var ver = System.BitConverter.GetBytes(vers);
                    s.Write(ver, 0, ver.Length);
                    log += "Текущая версия БД: " + vers + "\n";
                    var src = new BlockingCollection<KeyValuePair<byte, object>>();

                    log += "Запуск процесса поиска сущностей текущей дельты\n";

                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        ProcessEntitySearch(keys, container, master, src, masterHelper, companyId);
                    });

                    log += "Запуск процесса записи потока сущностей текущей дельты\n";
                    log += "UTCCORR: " + utcCorr + "\n";
                    int n = 0;
                    foreach(var item in src.GetConsumingEnumerable())
                    {
                        n++;
                        CorrectItemUtc(item.Value, utcCorr, ref log);
                        BinaryFormatter ser = new BinaryFormatter();
                        var ms = new MemoryStream();
                        var zs = new GZipStream(ms, CompressionMode.Compress);
                        ser.Serialize(zs, item);
                        zs.Close();
                        var arr = ms.ToArray();
                        var h = System.BitConverter.GetBytes(arr.Length);
                        s.Write(h, 0, h.Length);
                        s.Write(arr, 0, arr.Length);
                    }
                    Debug.WriteLine("Total objects: " + n);
                    log += "Процесс записи потока сущностей текущей дельты завершен\n";
                }
                Debug.WriteLine("Total stream length: " + s.Length);
                s.Position = 0;
                return s;
            }
            catch(Exception ex)
            {
                log += "Сбой при получении потока данных дельты.\n" + ex.Message + "\n" + ex.StackTrace + "\n";
                throw ex;
            }
        }

        private static Type[] TypesToIgnoreCorrection = new Type[] { typeof(Anket) };

        private static void CorrectItemUtc(object item, int utcCorr, ref string log)
        {
            if(TypesToIgnoreCorrection.Contains(item.GetType()))
            {
                return;
            }
            foreach(var prop in item.GetType().GetProperties())
            {
                try
                {
                    if(prop.PropertyType == typeof(DateTime?))
                    {
                        var val = (DateTime?)prop.GetValue(item, null);
                        if(val == null) return;
                        prop.SetValue(item, val.Value.AddHours(utcCorr), null);
                        return;
                    }
                    if(prop.PropertyType == typeof(DateTime) && prop.CanWrite)
                    {
                        var val = (DateTime)prop.GetValue(item, null);
                        if(val > DateTime.MinValue)
                        {
                            prop.SetValue(item, val.AddHours(utcCorr), null);
                        }
                    }
                }
                catch(Exception ex)
                {
                    if(DateTime.MinValue.Equals(prop.GetValue(item, null))) continue;
                    log += "Объект: " + item.ToString() + "\nСвойство: " + prop.Name + "\n";
                    try
                    {
                        log += String.Format("Значение: {0}\n", prop.GetValue(item, null));
                    }
                    catch { }
                    throw ex;
                }
            }
        }

        public static void ProcessEntitySearch(DataSet keys,
            EntityContainer container,
            ObjectContext master,
            BlockingCollection<KeyValuePair<byte, object>> src,
            ObjectContext masterHelper,
            Guid companyId)
        {
            Stopwatch a1 = Stopwatch.StartNew();
            try
            {
                var tnum = 0;
                foreach(DataTable dt in keys.Tables)
                {
                    Console.WriteLine("Table: " + (tnum++) + " Rows: " + dt.Rows.Count);
                    Stopwatch sw = Stopwatch.StartNew();
                    if(dt.Rows.Count == 0) continue;
                    var tname = FixEntSet(dt.Rows[0][0].ToString());
                    if(container.BaseEntitySets.Any(i => i.Name == tname && i.BuiltInTypeKind == BuiltInTypeKind.EntitySet))
                    {
                        for(int i = 0; i < dt.Rows.Count; i++)
                        {
                            if(i % 15000 == 0 && i > 0) Console.WriteLine("Row " + i);
                            var keyData = new List<KeyValuePair<string, object>>();
                            for(int j = 1; j < dt.Columns.Count; j++)
                            {
                                if(dt.Columns[j].ColumnName != "ChType")
                                {
                                    Guid val;
                                    if(Guid.TryParse(dt.Rows[i][j].ToString(), out val))
                                    {
                                        keyData.Add(new KeyValuePair<string, object>(FixKey(dt.Columns[j].ColumnName),
                                            val));
                                    }
                                    else
                                    {
                                        keyData.Add(new KeyValuePair<string, object>(FixKey(dt.Columns[j].ColumnName),
                                            Int32.Parse(dt.Rows[i][j].ToString())));
                                    }
                                }
                            }

                            EntityKey key = new System.Data.EntityKey("ExtraEntities." + tname, keyData);
                            object obj;
                            if(master.TryGetObjectByKey(key, out obj))
                            {
                                master.Detach(obj);
                                src.Add(new KeyValuePair<byte, object>(GetChangeType(dt.Rows[i]), obj));
                            }
                            else
                            {
                                src.Add(new KeyValuePair<byte, object>(3, key));
                            }
                        }
                    }
                    else
                    {
                        var item = container.BaseEntitySets.First(j => j.Name == tname && j.BuiltInTypeKind == BuiltInTypeKind.AssociationSet);
                        var ends = item.MetadataProperties["AssociationSetEnds"].Value as IEnumerable<MetadataItem>;
                        var entSet1 = ends.First().MetadataProperties["EntitySet"].Value as EntitySetBase;
                        var entSet2 = ends.Last().MetadataProperties["EntitySet"].Value as EntitySetBase;

                        var set1snap = true;
                        var set2snap = true;

                        for(int i = 0; i < dt.Rows.Count; i++)
                        {
                            if(i % 15000 == 0 && i > 0) Console.WriteLine("Row " + i);
                            if((set1snap && TestRelatedObject(masterHelper, entSet1, dt.Rows[i][1], companyId, ref set1snap)) ||
                                (set1snap && TestRelatedObject(masterHelper, entSet1, dt.Rows[i][2], companyId, ref set1snap)) ||
                                (set2snap && TestRelatedObject(masterHelper, entSet2, dt.Rows[i][1], companyId, ref set2snap)) ||
                                (set2snap && TestRelatedObject(masterHelper, entSet2, dt.Rows[i][2], companyId, ref set2snap)))
                                continue;

                            src.Add(new KeyValuePair<byte, object>(GetChangeType(dt.Rows[i]), new RelationInfo
                            {
                                RelationName = FixEntSetBack(tname),
                                LeftName = dt.Columns[1].ColumnName,
                                RightName = dt.Columns[2].ColumnName,
                                Left = Guid.Parse(dt.Rows[i][1].ToString()),
                                Right = Guid.Parse(dt.Rows[i][2].ToString())
                            }));

                        }
                    }
                    sw.Stop();
                    Debug.WriteLine("ProcessEntitySearch " + tname + ", rows " + dt.Rows.Count + ", takes " + sw.ElapsedMilliseconds);
                }
                //log += "Процесс поиска сущностей текущей дельты завершен успешно\n";
                src.CompleteAdding();
            }
            catch(Exception ex)
            {
                //log += "Процесс поиска сущностей текущей дельты завершен с ошибкой.\n" + ex.Message + "\n" + ex.StackTrace + "\n";
                throw ex;
            }
            a1.Stop();
            Debug.WriteLine("PES takes " + a1.ElapsedMilliseconds);
        }

        private static byte GetChangeType(DataRow row)
        {
            if(row.Table.Columns.Contains("ChType"))
            {
                var p = row["ChType"];
                if(p == null || ((string)p).ToUpper() == "I")
                {
                    return 1;
                }
                if(((string)p).ToUpper() == "U")
                {
                    return 2;
                }
                //Delete
                return 3;
            }
            else
            {
                return 1;
            }
        }

        private static bool TestRelatedObject(ObjectContext context, EntitySetBase entitySet, object keyValue, Guid companyId, ref bool setSnap)
        {
            object related;
            if(null != (related = context.GetByKey(entitySet.Name, entitySet.ElementType.KeyMembers[0].Name, keyValue)))
            {
                var pi = related.GetType().GetProperty("CompanyId");
                if(pi == null)
                {
                    Debug.WriteLine("SetSnap set to false for " + related.GetType().FullName);
                    setSnap = false;
                    return false;
                }

                var compId = related.GetValue("CompanyId");
                if(compId != null && !companyId.Equals(compId)) return true;
            }
            return false;
        }

        public static long GetCurrentChangeTrackingVersion(ExtraEntities context)
        {
            using(var conn = new SqlConnection(((EntityConnection)context.Connection).StoreConnection.ConnectionString))
            {
                conn.Open();
                return (long)new SqlCommand
                {
                    CommandText = "Select isnull(CHANGE_TRACKING_CURRENT_VERSION(),0)",
                    CommandType = CommandType.Text,
                    Connection = conn
                }.ExecuteScalar();
            }
        }

        public static long CommitStream(Stream stream, ref string log)
        {
            var lst = new List<KeyValuePair<byte, object>>();
            var ms = new MemoryStream();

            int buffLength = 1000;
            var buff = new byte[buffLength];
            var bytesRead = 0;

            while((bytesRead = stream.Read(buff, 0, buffLength)) > 0)
            {
                ms.Write(buff, 0, bytesRead);
            }

            stream.Close();

            //ms.Position = 0;

            //FileStream file = new FileStream("c:\\temp\\client\\" + DateTime.Now.ToString().Replace(":", "_") + ".bin", FileMode.Create, System.IO.FileAccess.Write);
            //byte[] bytes = new byte[ms.Length];
            //ms.Read(bytes, 0, (int)ms.Length);
            //file.Write(bytes, 0, bytes.Length);
            //file.Close();

            log += "Поток прочитан, длина: " + ms.Position + "\n";

            ms.Position = 0;
            var h1 = new byte[8];
            ms.Read(h1, 0, 8);
            var ver = System.BitConverter.ToInt64(h1, 0);
            var h = new byte[4];
            while(true)
            {
                if(ms.Read(h, 0, 4) == 0) break;
                var len = System.BitConverter.ToInt32(h, 0);
                if(len > 0)
                {
                    var arr = new byte[len];
                    ms.Read(arr, 0, len);
                    var ms1 = new MemoryStream(arr);
                    var zs = new GZipStream(ms1, CompressionMode.Decompress);

                    var bf = new BinaryFormatter();
                    lst.Add((KeyValuePair<byte, object>)bf.Deserialize(zs));

                    zs.Close();
                    ms1.Dispose();
                }
            }

            log += "Поток десериализован, всего сущностей: " + lst.Count + "\n";

            using(var local = new ExtraClub.Entities.ExtraEntities())
            {
                using(var conn = new SqlConnection(((EntityConnection)local.Connection).StoreConnection.ConnectionString))
                {
                    conn.Open();
                    log += "Соединение с БД АСУ установлено\n";
                    var container = local.MetadataWorkspace.GetEntityContainer(local.DefaultContainerName, System.Data.Metadata.Edm.DataSpace.CSpace);
                    log += "Контейнер метаданных сущностей получен\n";
                    foreach(var pair in lst.Where(i => i.Value is RelationInfo && i.Key == 3))
                    {
                        var rel = pair.Value as RelationInfo;

                        new SqlCommand(String.Format("Delete from {0} where {1}=@l and {2}=@r", rel.RelationName, rel.LeftName, rel.RightName), conn) { CommandType = System.Data.CommandType.Text }
                            .AddParameter<Guid>("l", rel.Left)
                            .AddParameter<Guid>("r", rel.Right)
                            .ExecuteNonQuery();

                    }

                    log += "Удаление связей 'много-много' выполнено\n";
                    var xl = 0;
                    foreach(var pair in lst.Where(i => !(i.Value is RelationInfo)))
                    {
                        if((xl++) % 1013 == 0)
                        {
                            Console.Write(" " + (xl));
                        }
                        var i = pair.Value;
                        var tName = i.GetType().FullName.Replace("ServiceModel", "Entities");
                        var set = container.BaseEntitySets.FirstOrDefault(j => j.ElementType.FullName == tName);
                        if(set != null)
                        {
                            if(pair.Key == 2)
                            {
                                //Update object
                                UpdateObject(local, set.Name, i, ref log);
                            }
                            else
                            {
                                //Add object
                                try
                                {
                                    var keyData = new List<KeyValuePair<string, object>>();
                                    var keyName = GetKeyNameByObjectType(i.GetType());
                                    if(i.GetValue(keyName) is Guid)
                                    {
                                        keyData.Add(new KeyValuePair<string, object>(keyName, (Guid)i.GetValue(keyName)));
                                    }
                                    else
                                    {
                                        keyData.Add(new KeyValuePair<string, object>(keyName, (int)i.GetValue(keyName)));
                                    }
                                    log += String.Format("[{0}={1}]\n", keyName, i.GetValue(keyName));
                                    EntityKey key = new System.Data.EntityKey("ExtraEntities." + set.Name, keyData);
                                    object obj;
                                    if(local.TryGetObjectByKey(key, out obj))
                                    {
                                        log += String.Format("(+) объект {1} с ключом {0} уже присутствует в бд\n", i.GetValue(keyName), set.Name);
                                        ApplyDiffereces(i, obj, ref log);
                                    }
                                    else
                                    {
                                        log += String.Format("(+) объект {1} с ключом {0} добавляется\n", i.GetValue(keyName), set.Name);
                                        local.AddObject(set.Name, i);
                                    }
                                }
                                catch(Exception exc)
                                {
                                    Console.WriteLine(String.Format("(+) объект {0} не получилось взять. Добавляем.\n", set.Name));
                                    Console.ReadLine();
                                    log += String.Format("(+) объект {0} не получилось взять. Добавляем.\n", set.Name);
                                    log += String.Format("{0}\n{1}\n", exc.GetType().Name, exc.Message);
                                    try
                                    {
                                        log += GetKeyNameByObjectType(i.GetType());
                                    }
                                    catch { }
                                    //log += "Некритичное исключение при добавлении " + set.Name + ":\n";
                                    //log += exc.Message + "\n";
                                    local.AddObject(set.Name, i);
                                }
                            }
                        }
                        else
                        {
                            if(pair.Key == 3)
                            {
                                //Remove object
                                object obj;
                                if(local.TryGetObjectByKey((EntityKey)i, out obj))
                                {
                                    local.DeleteObject(obj);
                                }
                                //local.AttachTo(set.Name, pair.Value);
                                //local.ObjectStateManager.ChangeObjectState(pair.Value, EntityState.Deleted);
                            }
                        }
                        //TODO: REMOVE!!!!!
                        //local.SaveChanges();
                        //////;
                    }

                    log += "Сущности добавлены/изменены/удалены\n";


                    try
                    {
                        local.SaveChanges();
                    }
                    catch(UpdateException ex)
                    {
                        foreach(var se in ex.StateEntries)
                        {
                            if(se != null && se.EntityKey != null && se.EntityKey.EntityKeyValues != null)
                            {
                                log += String.Format("Ошибка при обновлении сущности с ключом {0} в таблице {1}\n", String.Join(";", se.EntityKey.EntityKeyValues.Select(i => i.Value).Where(i => i != null)), se.EntitySet.Name);
                            }
                            else if(se != null && se.EntitySet != null)
                            {
                                log += String.Format("Ошибка при обновлении сущности в таблице {0}\n", se.EntitySet.Name);
                            }
                        }
                        throw ex;
                    }
                    catch(Exception ex)
                    {
                        if(ex.InnerException != null)
                        {
                            log += "Исключение при сохранении данных.\n" + ex.Message + "\n" + ex.InnerException.Message;
                        }
                        else
                        {
                            log += "Исключение при сохранении данных.\n" + ex.Message;
                        }
                        throw new Exception(String.Format("Исключение при сохранении данных"), ex);
                    }

                    log += "Изменения сохранены в БД\n";
                    foreach(var pair in lst.Where(i => i.Value is RelationInfo && i.Key == 1))
                    {
                        var rel = pair.Value as RelationInfo;

                        try
                        {
                            log += String.Format("insert into {0}({1},{2}) select @l, @r where not exists (Select * from {0} where {1}=@l and {2}=@r)\n", rel.RelationName, rel.LeftName, rel.RightName);
                            log += String.Format("@l={0}, @r={1}", rel.Left, rel.Right);
                            new SqlCommand(String.Format("insert into {0}({1},{2}) select @l, @r where not exists (Select * from {0} where {1}=@l and {2}=@r)", rel.RelationName, rel.LeftName, rel.RightName), conn) { CommandType = System.Data.CommandType.Text }
                                .AddParameter<Guid>("l", rel.Left)
                                .AddParameter<Guid>("r", rel.Right)
                                .ExecuteNonQuery();

                        }
                        catch(Exception ex)
                        {
                            if(ex.InnerException != null)
                            {
                                log += "Исключение при сохранении связей 'много-много'.\n" + ex.Message + "\n" + ex.InnerException.Message;
                            }
                            throw new Exception("Исключение при сохранении связей 'много-много'", ex);
                        }
                    }
                    log += "Добавление связей 'много-много' выполнено\n";
                }
            }
            return ver;
        }

        internal static string GetKeyNameByObjectType(Type type)
        {
            if(type == typeof(User)) return "UserId";
            if(type == typeof(Permission)) return "PermissionId";
            if(type == typeof(Role)) return "RoleId";
            if(type == typeof(Company)) return "CompanyId";
            return "Id";
        }

        internal static void UpdateObject(ExtraEntities local, string setName, object obj, ref string log)
        {
            log += String.Format("(^) объект {1} с ключом {0}\n", obj.GetValue(GetKeyNameByObjectType(obj.GetType())), setName);
            //Console.WriteLine(String.Format("(^) объект {1} с ключом {0}\n", obj.GetValue(GetKeyNameByObjectType(obj.GetType())), setName));
            if(obj is User)
            {
                SyncUser(local, obj as User, ref log);
                return;
            }

            //Проверить, а нет ли уже слчаем такой сущности? Если нет, то стейтменеджер должен указывать "Added"
            //bool add = true;
            //bool objExists = true;
            if(obj.GetValue(GetKeyNameByObjectType(obj.GetType())) != null && setName != null)
            {
                try
                {
                    var keyData = new List<KeyValuePair<string, object>>();
                    keyData.Add(new KeyValuePair<string, object>(GetKeyNameByObjectType(obj.GetType()), (Guid)obj.GetValue(GetKeyNameByObjectType(obj.GetType()))));

                    EntityKey key = new System.Data.EntityKey("ExtraEntities." + setName, keyData);
                    object obja = null;
                    if(local.TryGetObjectByKey(key, out obja))
                    {
                        log += String.Format(" объект уже присутствует в бд, применяем изменения\n");
                        ApplyDiffereces(obj, obja, ref log);
                        return;
                    }
                    else
                    {
                        log += String.Format(" объект не обнаружен. Добавление.\n");
                        local.AddObject(setName, obj);
                        return;
                    }
                }
                catch(Exception ex)
                {
                    log += ex.Message + "\n";
                }
            }
            log += String.Format(" что-то пошло не так. идем по стандартной схеме.", obj.GetValue(GetKeyNameByObjectType(obj.GetType())));
            local.AttachTo(setName, obj);
            local.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
            //if (add)
            //{
            //    try
            //    {
            //        log += "Try add... ";
            //        if (obja != null && objExists)
            //        {
            //            local.DeleteObject(obja);
            //        }

            //        local.AttachTo("ExtraEntities." + setName, obj);
            //        if (objExists)
            //        {
            //            local.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
            //        }
            //        log += "Ok!\n";

            //    }
            //    catch (InvalidOperationException ex) {
            //        log += "Exception on add!:\n"+ex.Message+"\n";
            //    }
            //}
        }

        internal static void ApplyDiffereces(object src, object local, ref string log)
        {
            var srcCid = src.GetValue("CompanyId");
            var locCid = local.GetValue("CompanyId");
            if(srcCid != null && locCid != null && !srcCid.Equals(locCid))
            {
                log += String.Format("объекты отличаются свойством CompanyId. Пропускаем.\n");
                return;
            }

            var props = src.GetType().GetProperties();
            var res = false;
            foreach(var prop in props)
            {
                if(prop.Name == "Deleted" || prop.GetCustomAttributes(typeof(DataMemberAttribute), false).Length != 1) continue;
                var v1 = src.GetValue(prop.Name);
                var v2 = local.GetValue(prop.Name);
                if(PropertyChangeNeeded(src.GetType(), v1, v2))
                {
                    log += String.Format(" объекты отличаются свойством {0} ( {1} и {2})\n", prop.Name, src.GetValue(prop.Name) ?? (object)"", local.GetValue(prop.Name) ?? (object)"");
                    res = true;
                    try
                    {
                        local.SetValue(prop.Name, v1);
                    }
                    catch(ArgumentException)
                    {
                        log += String.Format("Attempt failed: {0}={1}", prop.Name, v1);
                        if(prop.Name != "EndTime")
                        {
                            throw;
                        }
                    }
                }
            }
            if(!res)
            {
                log += String.Format(" oбъекты не отличаются\n");
            }
            //return false;
        }

        static HashSet<Type> NoNullOverwritingTypes = new HashSet<Type> { typeof(Customer), typeof(CustomerVisit), typeof(CustomerShelf) };

        private static bool PropertyChangeNeeded(Type type, object destination, object local)
        {

            if(NoNullOverwritingTypes.Contains(type))
            {
                return (destination != null && local == null)
                    || (destination != null && !destination.Equals(local))
                    || (local != null && !local.Equals(destination));
            }
            return (destination == null && local != null)
                || (destination != null && local == null)
                || (destination != null && !destination.Equals(local))
                || (local != null && !local.Equals(destination));
        }

        private static void SyncUser(ExtraEntities context, User user, ref string log)
        {
            var curr = context.Users.SingleOrDefault(i => i.UserId == user.UserId);
            user.UserName = user.UserName.ToLower();
            if(curr == null && !context.Users.Any(u => u.UserName == user.UserName))
            {
                log += String.Format(" пользователь {0} не обнаружен, добавляем\n", user.UserId);
                context.Users.AddObject(user);
                return;
            }
            if((user.LastPasswordChanged.HasValue && !curr.LastPasswordChanged.HasValue) ||
                (user.LastPasswordChanged > curr.LastPasswordChanged))
            {
                log += String.Format(" смена пароля для пользователя {0}\n", user.UserName);
                curr.PasswordHash = user.PasswordHash;
                curr.LastPasswordChanged = user.LastPasswordChanged;
            }
            else
            {
                log += String.Format(" смена пароля для пользователя {0} не требуется. локально пароль менялся {1}, удаленно - {2}\n",
                    user.UserName,
                    curr.LastPasswordChanged,
                    user.LastPasswordChanged);
            }
            curr.FullName = user.FullName;
            curr.IsActive = user.IsActive;
            curr.Email = user.Email;
        }

        public static string GetSystemId()
        {
            var sysInfo = new StringBuilder();
            var mc = new ManagementClass("Win32_Processor");
            var moc = mc.GetInstances();
            foreach(ManagementObject mo in moc)
            {
                sysInfo.Append(mo.Properties["ProcessorId"].Value.ToString());
            }

            //mc = new ManagementClass("Win32_DiskDrive");
            //moc = mc.GetInstances();
            //foreach (ManagementObject mo in moc)
            //{
            //    if (sysInfo != null && mo != null && mo.Properties != null && mo.Properties["SerialNumber"] != null && mo.Properties["SerialNumber"].Value != null)
            //        sysInfo.Append(mo.Properties["SerialNumber"].Value.ToString());
            //}

            sysInfo.Append(Environment.MachineName);

            return AdminCore.CalculateSHA1(sysInfo.ToString());
        }

        public static long GetVersion()
        {
            using(var context = new ExtraEntities())
            {
                var sets = context.LocalSettings.SingleOrDefault();
                if(sets == null)
                {
                    sets = new LocalSetting { Id = 1, DbVersion = -1, LastSyncVersion = -1 };
                    context.LocalSettings.AddObject(sets);
                    context.SaveChanges();
                }
                return sets.LastSyncVersion;
            }
        }

        public static void SetVersion(long version)
        {
            using(var context = new ExtraEntities())
            {
                var sets = context.LocalSettings.SingleOrDefault();
                if(sets == null)
                {
                    sets = new LocalSetting { Id = 1, DbVersion = -1, LastSyncVersion = version };
                    context.LocalSettings.AddObject(sets);
                }
                sets.LastSyncVersion = version;
                sets.LastSyncDate = DateTime.Now;
                context.SaveChanges();
            }
        }
    }
}
