using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.Infrastructure.ParameterClasses
{
    public class NewSolariumVisitParams
    {
        public Guid CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public Guid SolariumId { get; set; }

        public Action CloseAction { get; set; }
    }
}
