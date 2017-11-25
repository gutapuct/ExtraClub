using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TonusClub.ServiceModel;
using System.ServiceModel;
using System.Configuration;
using TonusClub.ServiceModel.Schedule;
using System.ServiceModel.Security;

namespace NewtonicServer
{
    static class ClientContext
    {
        static ITonusService client;
        static ChannelFactory<ITonusService> cf;
        static ClientContext()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(IgnoreCertificateErrorHandler);
            cf = new ChannelFactory<ITonusService>("TCEndpoint");

            cf.Endpoint.Address = new EndpointAddress(ConfigurationManager.AppSettings.Get("ServerAddress"));

            if (cf.Credentials != null)
            {
                cf.Credentials.UserName.UserName = ConfigurationManager.AppSettings.Get("Login");
                cf.Credentials.UserName.Password = ConfigurationManager.AppSettings.Get("Password");
            }

            client = cf.CreateChannel();
        }

        public static bool IgnoreCertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        internal static Treatment GetTreatmentBySerial(string macAddr)
        {
            try
            {
                return client.GetTreatmentByMac(macAddr);
            }
            catch (Exception exception)
            {
                if (((ICommunicationObject)client).State != CommunicationState.Faulted) throw exception;
                client = cf.CreateChannel();
                return client.GetTreatmentByMac(macAddr);
            }
        }

        internal static Customer GetCustomerByCard(int cardnum)
        {
            Customer res;
            try
            {
                res = client.GetCustomerByCard(cardnum, false);
            }
            catch (Exception exception)
            {
                if (((ICommunicationObject)client).State != CommunicationState.Faulted) throw exception;
                client = cf.CreateChannel();
                try
                {
                    res = client.GetCustomerByCard(cardnum, false);
                }
                catch
                {
                    return null;
                }
            }
            if (res == null) return null;
            List<CustomerVisit> viss;
            try
            {
                viss = client.GetCustomerVisits(res.Id);
            }
            catch (Exception exception)
            {
                if (((ICommunicationObject)client).State != CommunicationState.Faulted) throw exception;
                client = cf.CreateChannel();
                try
                {
                    viss = client.GetCustomerVisits(res.Id);
                }
                catch
                {
                    return null;
                }
            }
            if (!viss.Any(i => !i.OutTime.HasValue))
            {
                return null;
            }
            return res;
        }

        internal static TreatmentEvent GetCustomerPlanningForTreatment(Guid customerId, Guid treatmentId)
        {
            try
            {
                return client.GetCustomerPlanningForTreatment(customerId, treatmentId);
            }
            catch (Exception exception)
            {
                if (((ICommunicationObject)client).State != CommunicationState.Faulted) throw exception;
                client = cf.CreateChannel();
                return client.GetCustomerPlanningForTreatment(customerId, treatmentId);
            }
        }

        internal static void PostTreatmentStart(Guid treatmentId, DateTime newTime)
        {
            try
            {
                client.PostTreatmentStart(treatmentId, newTime);
            }
            catch (Exception exception)
            {
                if (((ICommunicationObject)client).State != CommunicationState.Faulted) throw exception;
                client = cf.CreateChannel();
                client.PostTreatmentStart(treatmentId, newTime);
            }
        }

        internal static HWProposal GetProposal(Guid treatmentId, Guid customerId)
        {
            try
            {
                return client.GetHWProposal(treatmentId, customerId);
            }
            catch (Exception exception)
            {
                if (((ICommunicationObject)client).State != CommunicationState.Faulted) throw exception;
                client = cf.CreateChannel();
                return client.GetHWProposal(treatmentId, customerId);
            }
        }

        internal static List<Treatment> GetAllTreatments()
        {
            try
            {
                return client.GetAllTreatments(Guid.Parse(ConfigurationManager.AppSettings.Get("DivisionId")));
            }
            catch (Exception exception)
            {
                if (((ICommunicationObject)client).State != CommunicationState.Faulted) throw exception;
                client = cf.CreateChannel();
                return client.GetAllTreatments(Guid.Parse(ConfigurationManager.AppSettings.Get("DivisionId")));
            }
        }

        internal static TreatmentEvent PostNewTreatmentEvent(Guid ticketId, Guid treatmentId, DateTime visitStart)
        {
            try
            {
                return client.PostNewTreatmentEvent(ticketId, treatmentId, visitStart);
            }
            catch (Exception exception)
            {
                if (((ICommunicationObject)client).State != CommunicationState.Faulted) throw exception;
                client = cf.CreateChannel();
                return client.PostNewTreatmentEvent(ticketId, treatmentId, visitStart);
            }
        }

        internal static void SetTreatmentOnline(Guid treatmnetId, bool isOnline)
        {
            try
            {
                client.SetTreatmentOnline(treatmnetId, isOnline);
            }
            catch (Exception exception)
            {
                if (((ICommunicationObject)client).State != CommunicationState.Faulted) throw exception;
                client = cf.CreateChannel();
                client.SetTreatmentOnline(treatmnetId, isOnline);
            }
        }

        internal static TreatmentEvent GetCurrentTreatmentEvent(Guid treatmentId)
        {
            try
            {
                return client.GetCurrentTreatmentEvent(treatmentId);
            }
            catch (Exception exception)
            {
                if (((ICommunicationObject)client).State != CommunicationState.Faulted) throw exception;
                client = cf.CreateChannel();
                return client.GetCurrentTreatmentEvent(treatmentId);
            }
        }

        internal static int CorrectAvailableTreatmentLength(Guid eventId)
        {
            try
            {
                return client.CorrectAvailableTreatmentLength(eventId);
            }
            catch (Exception exception)
            {
                if (((ICommunicationObject)client).State != CommunicationState.Faulted) throw exception;
                client = cf.CreateChannel();
                return client.CorrectAvailableTreatmentLength(eventId);
            }
        }
    }
}
