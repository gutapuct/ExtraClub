using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sync.Models
{
    public class RegionalServerView
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string DivisionName { get; set; }
        public DateTime? LastSync { get; set; }
        public DateTime? FirstSync { get; set; }
        public DateTime? LastKeyReceived { get; set; }

        public string HWKey { get; set; }

        public Guid CompanyId { get; set; }
        public Guid DivisionId { get; set; }

        public string EmailSuccess { get; set; }
        public string EmailFailure { get; set; }

        public string ContactPerson { get; set; }

        public int MaxSyncPeriod { get; set; }
        public bool IsLicenseAvailable { get; set; }
        public bool SendSms { get; set; }
        public DateTime? LicenseTill { get; set; }
        public DateTime? LicenseTillUncommited { get; set; }

        public string Versions { get; set; }

        public DateTime? LastUpdated { get; set; }

        public DateTime? SmsUntil { get; set; }

        public bool SmsMarketing { get; set; }
        public bool SmsBirthday { get; set; }
        public bool SmsTreatments { get; set; }

    }

}