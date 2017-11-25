using System;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using TonusClub.ServiceModel;
using TonusClub.ServerCore;
using TonusClub.Entities;
using Sync.Models;
using System.Data.EntityClient;
using Sync;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Net.Mime;
using System.Diagnostics;
using Sync.Code;
using ClaimServiceContract;

public class Service : ISyncService
{
    static int CoherentVersion = 112;

    public Stream GetInitialSyncData(string machineKey)
    {
        using(var context = new SyncMetadataEntities())
        {
            string log = String.Empty;
            var company = context.MetaCompanies.SingleOrDefault(i => i.MachineKey == machineKey);
            if(company == null) throw new FaultException<string>("Аппаратный ключ не зарегистрирован!", "Аппаратный ключ не зарегистрирован!");
            try
            {
                SyncLogger.LogFormat(company.DivisionId, "Запрос первичной синхронизации");
                if(company.LastSyncDate.HasValue)
                {
                    SyncLogger.LogFormat(company.DivisionId, "Первичная синхронизация уже выполнялась. Перенаправление на GetServerPart");
                    return GetServerPart(machineKey, -1);
                }
                var comp = new TonusEntities().Companies.Single(i => i.CompanyId == company.CompanyId);
                return SyncCore.GetDataSetStream(company.CompanyId, company.LastSyncVersion ?? -1, (i, j) => GetInitialKeys(i), ref log, comp.UtcCorr);
            }
            catch(Exception ex)
            {
                log += "Исключение при генерации потока первичной синхронизации:\n" + ex.Message + "\n" + ex.StackTrace + "\n";
                NotificationCore.SendMail(new TonusEntities().Divisions.Single(i => i.Id == company.DivisionId), ex, company.EmailFailure, log);

                var ex1 = ex.InnerException;
                while(ex1 != null)
                {
                    log += ex1.GetType().ToString() + "\n" + ex1.Message + "\n" + ex1.StackTrace;
                    ex1 = ex1.InnerException;
                }

                SyncCore.ReportLog(log);

                throw ex;
            }
            finally
            {
                SyncLogger.LogFormat(company.DivisionId, log);
            }
        }
    }

    public Stream GetServerPart(string machineKey, int coherentVersion)
    {
        if(coherentVersion != CoherentVersion && coherentVersion > 0)
        {
            throw new FaultException<string>("Необходимо обновить версию регионального сервера!", "Необходимо обновить версию регионального сервера!");
        }
        using(var context = new SyncMetadataEntities())
        {
            string log = String.Empty;

            var company = context.MetaCompanies.SingleOrDefault(i => i.MachineKey == machineKey);
            if(company == null) throw new FaultException<string>("Аппаратный ключ не зарегистрирован!", "Аппаратный ключ не зарегистрирован!");
            var ld = Locker.IsLocked(company.CompanyId);
            if(ld.HasValue)
            {
                var msg = String.Format("Синхронизация заблокирована до {0:dd.MM.yyyy HH:mm}!", ld.Value);
                throw new FaultException<string>(msg, msg);
            }
            else
            {
                Locker.SetLock(company.CompanyId);
            }
            try
            {
                SyncLogger.LogFormat(company.DivisionId, "Запрос синхронизации");

                if(!company.LastSyncDate.HasValue)
                {
                    return GetInitialSyncData(machineKey);
                }

                var comp = new TonusEntities().Companies.Single(i => i.CompanyId == company.CompanyId);

                var resStream = SyncCore.GetDataSetStream(company.CompanyId, company.LastSyncVersion ?? -1, (i, j) => GetServerDelta(i, j), ref log, comp.UtcCorr);

                OperationContext clientContext = OperationContext.Current;
                clientContext.OperationCompleted += new EventHandler(delegate(object sender, EventArgs args)
                {
                    Debug.WriteLine("Stream is closed");
                    resStream.Close();
                });
                return resStream;
            }
            catch(Exception ex)
            {
                log += "Исключение при генерации потока дельты серверной стороны:\n" + ex.Message + "\n" + ex.StackTrace + "\n";

                NotificationCore.SendMail(new TonusEntities().Divisions.Single(i => i.Id == company.DivisionId), ex, company.EmailFailure, log);

                var ex1 = ex.InnerException;
                while(ex1 != null)
                {
                    log += ex1.GetType().ToString() + "\n" + ex1.Message + "\n" + ex1.StackTrace;
                    ex1 = ex1.InnerException;
                }

                SyncCore.ReportLog(log);


                throw ex;
            }
            finally
            {
                SyncLogger.LogFormat(company.DivisionId, log);
            }
        }
    }

    private DataSet GetServerDelta(Guid companyId, long version)
    {
        try
        {
            using(var conn = new SqlConnection(((EntityConnection)new TonusEntities().Connection).StoreConnection.ConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("sync_GetServerChanges", conn) { CommandType = System.Data.CommandType.StoredProcedure, CommandTimeout = 600 };
                cmd.Parameters.AddWithValue("company", companyId);
                cmd.Parameters.AddWithValue("version", version);
                var da = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }
        catch(Exception ex)
        {
            var log = "";
            var ex1 = ex;
            while(ex1 != null)
            {
                log += ex1.GetType().ToString() + "\n" + ex1.Message + "\n" + ex1.StackTrace;
                ex1 = ex1.InnerException;
            }

            SyncCore.ReportLog(log);
            throw ex;
        }
    }

    public DataSet GetInitialKeys(Guid companyId)
    {
        using(var conn = new SqlConnection((((EntityConnection)new TonusEntities().Connection).StoreConnection.ConnectionString)))
        {
            conn.Open();
            var cmd = new SqlCommand("sync_GetFirstIndex", conn) { CommandType = System.Data.CommandType.StoredProcedure };
            cmd.CommandTimeout = 600;
            cmd.Parameters.AddWithValue("companyId", companyId);
            var da = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
    }

    public long PostClientPart(Stream part)
    {
        var log = "";
        try
        {
            SyncCore.CommitStream(part, ref log);
            return SyncCore.GetCurrentChangeTrackingVersion(new TonusEntities());
        }
        catch(Exception ex)
        {
            var sw1 = new StreamWriter("c:\\temp\\" + DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + ".log", true);
            sw1.Write(("Исключение при записи потока дельты клиентской стороны:\n" + ex.Message + "\n" + ex.StackTrace + "\n" + log).Replace("\n", "\r\n"));
            sw1.Close();
            log += "Исключение при записи потока дельты клиентской стороны:\n" + ex.Message + "\n" + ex.StackTrace + "\n";
            //TODO: SendMail(null, ex, null, log);

            var ex1 = ex.InnerException;
            while(ex1 != null)
            {
                log += ex1.GetType().ToString() + "\n" + ex1.Message + "\n" + ex1.StackTrace;
                ex1 = ex1.InnerException;
            }

            SyncCore.ReportLog(log);


            throw ex;
        }
        finally
        {
            SyncLogger.LogFormat(Guid.Empty, log);
        }

    }

    public void PostSyncSuccess(string machineKey, long version)
    {
        try
        {
            using(var context = new SyncMetadataEntities())
            {
                var company = context.MetaCompanies.SingleOrDefault(i => i.MachineKey == machineKey);
                if(company == null) throw new FaultException<string>("Аппаратный ключ не зарегистрирован!", "Аппаратный ключ не зарегистрирован!");
                SyncLogger.LogFormat(company.DivisionId, "Клиент сообщил об успешном завершении синхронизации.\nВерсия серверной БД: " + version);
                company.LastSyncDate = DateTime.Now;
                Locker.Unlock(company.CompanyId);

                var otherDivs = context.MetaCompanies.Where(i => i.DivisionId != company.DivisionId && i.CompanyId == company.CompanyId && i.LastSyncVersion.HasValue && i.LastSyncVersion.Value > company.LastSyncVersion);
                if(otherDivs.Count() > 0)
                {
                    version = Math.Min(otherDivs.Min(i => i.LastSyncVersion ?? Int64.MaxValue), version);
                }

                company.LastSyncVersion = version;

                if(!company.FirstSync.HasValue) company.FirstSync = DateTime.Now;

                context.SaveChanges();
                NotificationCore.SendMail(new TonusEntities().Divisions.Single(i => i.Id == company.DivisionId), null, company.EmailSuccess);
            }
        }
        catch(Exception ex)
        {
            var log = "";
            var ex1 = ex.InnerException;
            while(ex1 != null)
            {
                log += ex1.GetType().ToString() + "\n" + ex1.Message + "\n" + ex1.StackTrace;
                ex1 = ex1.InnerException;
            }

            SyncCore.ReportLog(log);

        }
    }

    public byte[] GetLicenceKey(string machineKey)
    {
        using(var context = new SyncMetadataEntities())
        {
            var company = context.MetaCompanies.SingleOrDefault(i => i.MachineKey == machineKey);
            if(company == null) throw new FaultException<int>(0, "Аппаратный ключ не зарегистрирован!");
            if(company.MaxSyncPeriod < (DateTime.Today - (company.LastSyncDate ?? DateTime.MinValue)).TotalDays)
            {
                throw new FaultException<int>(1, "Необходимо провести синхронизацию региональной базы!");
            }
            //if(!company.IsLicenseAvailable)
            //{
            //    throw new FaultException<int>(2, "Лицензия заблокирована!");
            //}
            //if((company.LicenseTill ?? DateTime.MaxValue) < DateTime.Today)
            //{
            //    throw new FaultException<int>(3, "Лицензия истекла!");
            //}

            company.LastKeyReceived = DateTime.Now;
            context.SaveChanges();

            return Sign(GetLicenceDateForCompany(company) + " " + company.MachineKey);
        }
    }

    private string GetLicenceDateForCompany(MetaCompany company)
    {
        var date = DateTime.Today.AddDays(company.MaxSyncPeriod ?? 0);
        if((company.LicenseTill ?? DateTime.MaxValue) < date) date = company.LicenseTill.Value;
        return date.ToString("yyyy-MM-dd");
    }

    private static byte[] Sign(string text)
    {
        X509Store my = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        my.Open(OpenFlags.ReadOnly);

        RSACryptoServiceProvider csp = null;
        foreach(X509Certificate2 cert in my.Certificates)
        {
            if(cert.Thumbprint.Replace(" ", "").ToLower() == ConfigurationManager.AppSettings.Get("LicenseThumbprint"))
            {
                csp = (RSACryptoServiceProvider)cert.PrivateKey;
            }
        }
        if(csp == null)
        {
            throw new FaultException<int>(4, "Внутренняя ошибка сервера лицензирования №4");
        }

        SHA1Managed sha1 = new SHA1Managed();
        UnicodeEncoding encoding = new UnicodeEncoding();
        byte[] data = encoding.GetBytes(text);
        byte[] hash = sha1.ComputeHash(data);

        return csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
    }

    public DateTime GetExpiryDate(string machineKey)
    {
        using(var context = new SyncMetadataEntities())
        {
            var company = context.MetaCompanies.SingleOrDefault(i => i.MachineKey == machineKey);
            return company.LicenseTill ?? DateTime.MaxValue;
        }
    }

    public void PostApplicationErrorReport(string report)
    {
        using(var context = new SyncMetadataEntities())
        {
            var prop = OperationContext.Current.IncomingMessageProperties;
            var endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            var rep = new ErrorReports
            {
                CreatedOn = DateTime.Now,
                Id = Guid.NewGuid(),
                Message = report,
                ClientIp = endpoint.Address
            };
            context.ErrorReports.AddObject(rep);
            context.SaveChanges();
        }
    }


    public void PostFrReport(string email, string subj, string repDetails, byte[] repFile)
    {
        try
        {
            var message = new MailMessage();
            message.To.Add(email);
            message.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings.Get("SmtpFrom"), ConfigurationManager.AppSettings.Get("SmtpFromName"));
            message.Subject = subj;
            message.Body = repDetails;
            message.Attachments.Add(new Attachment(new MemoryStream(repFile), new ContentType("application/excel")) { Name = "Отчет.xls" });

            var smtp = new SmtpClient
            {
                Host = ConfigurationManager.AppSettings.Get("SmtpHost"),
                Port = Int32.Parse(ConfigurationManager.AppSettings.Get("SmtpPort"))
            };
            smtp.EnableSsl = ConfigurationManager.AppSettings.Get("UseSSL") == "1";
            smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings.Get("SmtpLogin"), ConfigurationManager.AppSettings.Get("SmtpPassword"));
            smtp.Send(message);
        }
        catch(Exception ex)
        {
            Logger.Log(ex);
            throw new FaultException<string>("Ошибка при отсылке письма", "Ошибка при отсылке письма");
        }
    }

    public Guid GetMainDivision(string machineKey)
    {
        using(var context = new SyncMetadataEntities())
        {
            var company = context.MetaCompanies.SingleOrDefault(i => i.MachineKey == machineKey);
            if(company == null) throw new FaultException<string>("Аппаратный ключ не зарегистрирован!", "Аппаратный ключ не зарегистрирован!");
            return company.DivisionId;
        }
    }

    public int PostNewClaim(Claim claim)
    {
        using(var context = new TonusEntities())
        {
            var cf = new ChannelFactory<IClaimService>("ClaimServiceEndpoint");
            var client = cf.CreateChannel();

            var cTest = new TonusEntities().Claims.SingleOrDefault(i => i.Id == claim.Id);
            if(cTest != null && cTest.FtmId.HasValue)
            {
                return cTest.FtmId.Value;
            } 
            if(cTest == null)
            {
                context.Claims.AddObject(claim);
            }
            else
            {
                claim = cTest;
            }
            var cName = context.Companies.Where(i => i.CompanyId == claim.CompanyId).Select(i => i.CompanyName).Single();
            var claimNumber = client.AddClaimEx(claim.Id, claim.ClaimTypeId, claim.CompanyId, cName, claim.ContactEmail, claim.ContactInfo,
                claim.ContactPhone, claim.CreatedOn, claim.Eq_BuyDate, claim.Eq_Guaranty, claim.Eq_Serial,
                claim.Message, claim.PrefFinishDate, claim.Subject, claim.Circulation, claim.Eq_TreatmentId,
                claim.Eq_TechContact, claim.Eq_SerialGutwell, claim.Eq_Model, claim.Eq_ClubAddr, claim.Eq_PostAddr);
            claim.FtmId = claimNumber;
            claim.StatusId = 2;
            claim.StatusDescription = "Запрос размещен в FTM";

            context.SaveChanges();

            return claimNumber;
        }
    }


    public void PostClaimSubmit(Guid claimId)
    {
        using(var context = new TonusEntities())
        {
            var cf = new ChannelFactory<IClaimService>("ClaimServiceEndpoint");
            var client = cf.CreateChannel();

            var claim = context.Claims.SingleOrDefault(i => i.Id == claimId);
            if(claim == null)
            {
                return;
            }
            if(claim.FtmId.HasValue)
            {
                client.SubmitClaim(claim.FtmId.Value);
            }
            claim.StatusId = 6;

            context.SaveChanges();
        }
    }

    public void PostClaimReopen(Guid claimId, string finishDescription)
    {
        using(var context = new TonusEntities())
        {
            var cf = new ChannelFactory<IClaimService>("ClaimServiceEndpoint");
            var client = cf.CreateChannel();

            var claim = context.Claims.SingleOrDefault(i => i.Id == claimId);
            if(claim == null)
            {
                return;
            }

            claim.FinishDescription = finishDescription;

            claim.FinishedByFtmId = null;
            claim.FinishedByName = null;
            claim.FinishDate = null;


            client.ReopenClaim(claim.FtmId.Value, finishDescription);
            claim.StatusId = 2;
            claim.StatusDescription = "Запрос возобновлен в FTM";

            context.SaveChanges();
        }
    }

    public Tuple<System.Collections.Generic.List<Customer>, System.Collections.Generic.List<CustomerNotification>> GetFromSite(Guid divisionId, DateTime fromTime)
    {
        using(var context = new TonusEntities())
        {
            context.ContextOptions.LazyLoadingEnabled = false;
            var customers = context.Customers.Include("CustomerNotifications").Where(i => i.ClubId == divisionId && i.FromSite && i.CreatedOn >= fromTime).ToList();
            var tasks = customers.SelectMany(i => i.CustomerNotifications).ToList();

            return Tuple.Create(customers, tasks);
        }
    }
}