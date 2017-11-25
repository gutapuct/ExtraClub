using System;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.IO;
using TonusClub.Entities;
using TonusClub.ServerCore;

namespace Flagmax.WorkflowService.CommandLine
{
    internal sealed class StreamCreator
    {
        public void Run(string[] args)
        {
            return;
            string log = "";
            var cId = Guid.Parse(args[1]);
            var ver = Int32.Parse(args[2]);

            Stream ms;
            if (ver > 0)
            {
                ms = SyncCore.GetDataSetStream(cId, ver, (id, ver1) => GetLocalDelta(new TonusEntities(), id, ver1), ref log, 0);
            }
            else
            {
                ms = SyncCore.GetDataSetStream(cId, ver, (id, ver1) => GetInitialKeys(id), ref log, 0);
            }
            using (var sw = new FileStream(Path.Combine(args[3], "stream.sync"), FileMode.Create, FileAccess.Write))
            {
                byte[] bytes = new byte[ms.Length];
                ms.Read(bytes, 0, (int)ms.Length);
                sw.Write(bytes, 0, bytes.Length);
                sw.Close();
            }
        }

        private static DataSet GetLocalDelta(TonusEntities context, Guid companyId, long version)
        {
            using (var conn = new SqlConnection(((EntityConnection)context.Connection).StoreConnection.ConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("sync_GetChanges", conn) { CommandType = CommandType.StoredProcedure };
                cmd.CommandTimeout = 600;
                cmd.Parameters.AddWithValue("company", companyId);
                cmd.Parameters.AddWithValue("version", version);
                var da = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                da.Fill(ds);

                return ds;
            }
        }

        private static DataSet GetInitialKeys(Guid companyId)
        {
            using (var conn = new SqlConnection((((EntityConnection)new TonusEntities().Connection).StoreConnection.ConnectionString)))
            {
                conn.Open();
                var cmd = new SqlCommand("sync_GetFirstIndex", conn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 };
                cmd.Parameters.AddWithValue("companyId", companyId);
                var da = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }
    }
}