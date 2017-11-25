using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ServiceModel;
using TonusClub.ServiceModel.HttpMethodsTypes;

namespace TonusClub.ServiceModel
{
    [ServiceContract]
    public interface IHttpMethods
    {
        [OperationContract]
        string GetZReport(string divisionId, string date);
        [OperationContract]
        string GetWorkTime(string divisionId, string year, string month);
        [OperationContract]
        string GetSalarySheet(string divisionId, string year, string month);
        [OperationContract]
        string GetPTU(string divisionId, string year, string month, string day);
    }
}
