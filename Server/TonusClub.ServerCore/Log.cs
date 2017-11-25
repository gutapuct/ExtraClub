using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using TonusClub.Entities;
using TonusClub.ServiceModel;

namespace TonusClub.ServerCore
{
    public static class Logger
    {
        static object _lockObj = new object();

        public static void Log(Exception ex)
        {
            Log("----------------------------------");
            Log(DateTime.Now.ToString());
            while (ex != null)
            {
                Log("Type: " + ex.GetType().ToString());
                Log("Message: " + ex.Message);
                Log("Trace: " + ex.StackTrace);
                ex = ex.InnerException;
                if (ex != null) Log("\nInner exception:\n");
            }
        }

        public static void Log(string str, params object[] args)
        {
            lock (_lockObj)
            {
                try
                {
                    var fold = new DirectoryInfo("c:\\temp\\");
                    if (!fold.Exists) fold.Create();
                    var f = new StreamWriter("c:\\temp\\tc.log", true);
                    f.WriteLine(String.Format(str, args));
                    f.Close();
                }
                catch { }
            }
        }

        public static void DBLog(string message, params object[] values)
        {
            try
            {
                MessageProperties prop = OperationContext.Current.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpoint =
                    prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                using (var context = new TonusEntities())
                {
                    var cId = UserManagement.GetCompanyIdOrDefaultId(context);
                    var log = new Log
                    {
                        Date = DateTime.Now,
                        HostIp = endpoint.Address,
                        Id = Guid.NewGuid(),
                        Message = String.Format(message, values),
                        UserName = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name,
                        CompanyId = cId
                    };
                    context.Logs.AddObject(log);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        public static void Log(StackTrace st)
        {
            foreach(var frame in st.GetFrames())
            {
                Log(frame.GetFileName() + ": line " + frame.GetFileLineNumber() + " (" + frame.GetMethod().Name + ")");
            }
        }
    }
}
