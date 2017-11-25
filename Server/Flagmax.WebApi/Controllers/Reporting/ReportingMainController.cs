using System.Collections.Generic;
using System.Web.Http;

namespace Flagmax.WebApi.Controllers.Reporting
{
    [Authorize]
    public class ReportingMainController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new[] {"1", "2", "3"};
        } 
    }
}
