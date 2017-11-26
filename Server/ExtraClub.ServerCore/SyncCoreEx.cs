using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.EntityClient;
using System.Data.SqlClient;
using ExtraClub.ServiceModel.Sync;
using System.Data;
using System.Collections.Concurrent;
using System.Threading;
using ExtraClub.ServiceModel;

namespace ExtraClub.ServerCore
{
    public class SyncCoreEx
    {
        public void CommitStreamEx(Stream ms)
        {
            var lst = new BlockingCollection<KeyValuePair<byte, object>>();
            var lstRel = new BlockingCollection<KeyValuePair<byte, object>>();

            ThreadPool.QueueUserWorkItem(_ =>
                {
                    DeserializeMethod(ms, lst, lstRel);
                });

            SaverMethod(lst);
            RelationSaverMethod(lstRel);

        }

        private void DeserializeMethod(Stream ms, BlockingCollection<KeyValuePair<byte, object>> lst, BlockingCollection<KeyValuePair<byte, object>> lstRel)
        {
            try
            {
                var h1 = new byte[8];
                ms.Read(h1, 0, 8);
                var ver = System.BitConverter.ToInt64(h1, 0);
                var h = new byte[4];
                int x = 0;
                while (true)
                {
                    if (x++ % 1000 == 0)
                    {
                        //Console.Write("*");
                        while (lst.Count > 10000)
                        {
                            Thread.Sleep(3000);
                            Console.WriteLine(String.Format("Десереализовано {0}, буфер сущностей {1}, буфер связей {2}", x, lst.Count, lstRel.Count));
                        }
                    }
                    if (ms.Read(h, 0, 4) == 0) break;
                    var len = System.BitConverter.ToInt32(h, 0);
                    if (len > 0)
                    {
                        var arr = new byte[len];
                        ms.Read(arr, 0, len);
                        var ms1 = new MemoryStream(arr);
                        var zs = new GZipStream(ms1, CompressionMode.Decompress);

                        var bf = new BinaryFormatter();
                        var pair = (KeyValuePair<byte, object>)bf.Deserialize(zs);
                        if (!(pair.Value is Kinds1C))
                        {
                            if (pair.Value is RelationInfo)
                            {
                                lstRel.Add(pair);
                            }
                            else
                            {
                                lst.Add(pair);
                            }
                        }
                        zs.Close();
                        ms1.Dispose();
                    }
                }

                Console.WriteLine("Поток десериализован "+x.ToString());

                lst.CompleteAdding();
                lstRel.CompleteAdding();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

        }


        private void SaverMethod(BlockingCollection<KeyValuePair<byte, object>> lst)
        {
            var local = new ExtraClub.Entities.ExtraEntities();
            var container = local.MetadataWorkspace.GetEntityContainer(local.DefaultContainerName, System.Data.Metadata.Edm.DataSpace.CSpace);
            local.ContextOptions.LazyLoadingEnabled = false;
            int counter = 0;
            KeyValuePair<byte, object> pair;
            while(lst.TryTake(out pair))
            {
                var i = pair.Value;
                var tName = i.GetType().FullName.Replace("ServiceModel", "Entities");
                var set = container.BaseEntitySets.FirstOrDefault(j => j.ElementType.FullName == tName);
                if (set != null)
                {
                    if (pair.Key == 2)
                    {
                        //Update object
                        string log = "";
                        SyncCore.UpdateObject(local, set.Name, i, ref log);
                    }
                    else
                    {
                        //Add object
                        try
                        {
                            var keyData = new List<KeyValuePair<string, object>>();
                            var keyName = SyncCore.GetKeyNameByObjectType(i.GetType());
                            if (i.GetValue(keyName) is Guid)
                            {
                                keyData.Add(new KeyValuePair<string, object>(keyName, (Guid)i.GetValue(keyName)));
                            }
                            else
                            {
                                keyData.Add(new KeyValuePair<string, object>(keyName, (int)i.GetValue(keyName)));
                            }
                            //Console.WriteLine(String.Format("[{0}={1}]\n", keyName, i.GetValue(keyName)));
                            EntityKey key = new System.Data.EntityKey("ExtraEntities." + set.Name, keyData);
                            object obj;
                            if (local.TryGetObjectByKey(key, out obj))
                            {
                                //Console.WriteLine(String.Format("(+) объект {1} с ключом {0} уже присутствует в бд\n", i.GetValue(keyName), set.Name));
                                string log = "";
                                SyncCore.ApplyDiffereces(i, obj, ref log);
                            }
                            else
                            {
                                //Console.WriteLine(String.Format("(+) объект {1} с ключом {0} добавляется\n", i.GetValue(keyName), set.Name));
                                local.AddObject(set.Name, i);
                            }
                        }
                        catch (Exception)
                        {
                            //Console.WriteLine(String.Format("(+) объект {0} не получилось взять. Добавляем.\n", set.Name));
                            //Console.WriteLine(String.Format("{0}\n{1}\n", exc.GetType().Name, exc.Message));
                            local.AddObject(set.Name, i);
                        }
                    }
                }
                else
                {
                    if (pair.Key == 3)
                    {
                        //Remove object
                        object obj;
                        if (local.TryGetObjectByKey((EntityKey)i, out obj))
                        {
                            local.DeleteObject(obj);
                        }
                    }
                }

                if (counter++ > 500)
                {
                    SaveLocal(local);
                    UpdateContext(ref local, ref container);
                    counter = 0;
                }
            }

            Console.WriteLine("Сущности добавлены/изменены/удалены\n");
        }

        private void UpdateContext(ref Entities.ExtraEntities local, ref System.Data.Metadata.Edm.EntityContainer container)
        {
            local = new ExtraClub.Entities.ExtraEntities();
            container = local.MetadataWorkspace.GetEntityContainer(local.DefaultContainerName, System.Data.Metadata.Edm.DataSpace.CSpace);
        }

        private void SaveLocal(Entities.ExtraEntities local)
        {
            try
            {
                local.SaveChanges();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Исключение при сохранении данных.\n" + ex.Message + "\n" + ex.InnerException.Message);
                }
                else
                {
                    Console.WriteLine("Исключение при сохранении данных.\n" + ex.Message);
                }
                throw new Exception(String.Format("Исключение при сохранении данных"), ex);
            }
            //Console.Write("S");
        }

        private void RelationSaverMethod(BlockingCollection<KeyValuePair<byte, object>> lstRel)
        {
            using (var local = new ExtraClub.Entities.ExtraEntities())
            {
                using (var conn = new SqlConnection(((EntityConnection)local.Connection).StoreConnection.ConnectionString))
                {
                    conn.Open();
                    Console.WriteLine("Соединение RelationSaver с БД АСУ установлено\n");

                    foreach (var pair in lstRel.GetConsumingEnumerable())
                    {
                        var rel = pair.Value as RelationInfo;
                        if (pair.Key == 3)
                        {
                            new SqlCommand(String.Format("Delete from {0} where {1}=@l and {2}=@r", rel.RelationName, rel.LeftName, rel.RightName), conn) { CommandType = System.Data.CommandType.Text }
                                .AddParameter<Guid>("l", rel.Left)
                                .AddParameter<Guid>("r", rel.Right)
                                .ExecuteNonQuery();
                        }
                        if (pair.Key == 1)
                        {
                            try
                            {
                                new SqlCommand(String.Format("insert into {0}({1},{2}) select @l, @r where not exists (Select * from {0} where {1}=@l and {2}=@r)", rel.RelationName, rel.LeftName, rel.RightName), conn) { CommandType = System.Data.CommandType.Text }
                                    .AddParameter<Guid>("l", rel.Left)
                                    .AddParameter<Guid>("r", rel.Right)
                                    .ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                if (ex.InnerException != null)
                                {
                                    Console.WriteLine("Исключение при сохранении связей 'много-много'.\n" + ex.Message + "\n" + ex.InnerException.Message);
                                }
                                throw new Exception("Исключение при сохранении связей 'много-много'", ex);
                            }
                        }
                    }
                }
            }
        }
    }
}
