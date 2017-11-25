using System;
using System.ServiceModel.Web;
using TonusClub.ServerCore;
using TonusClub.ServiceModel;

public class HttpMethods : IHttpMethods
{
    [WebGet(UriTemplate = "/GetZReport/{divisionId}/{date}")]
    public string GetZReport(string divisionId, string date)
    {
        return HttpMethodsCore.GetZReport(Guid.Parse(divisionId), DateTime.Parse(date));
    }

    [WebGet(UriTemplate = "/GetWorkTime/{divisionId}/{year}/{month}")]
    public string GetWorkTime(string divisionId, string year, string month)
    {
        return HttpMethodsCore.GetWorkTime(Guid.Parse(divisionId), new DateTime(Int32.Parse(year), Int32.Parse(month), 1));
    }

    [WebGet(UriTemplate = "/GetSalarySheet/{divisionId}/{year}/{month}")]
    public string GetSalarySheet(string divisionId, string year, string month)
    {
        return HttpMethodsCore.GetSalarySheet(Guid.Parse(divisionId), new DateTime(Int32.Parse(year), Int32.Parse(month), 1));
    }

    [WebGet(UriTemplate = "/GetPTU/{divisionId}/{year}/{month}/{day}")]
    public string GetPTU(string divisionId, string year, string month, string day)
    {
        return HttpMethodsCore.GetPTU(Guid.Parse(divisionId), new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day)));
    }
}