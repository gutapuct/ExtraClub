using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using TonusClub.ServiceModel.Reports;

namespace TonusClub.ServiceModel
{
    partial class ReportRecurrency
    {
        [DataMember]
        public string SerializedName { get; set; }

        public string RecurrencyText
        {
            get
            {
                return Recurrencies[Recurrency] + ", в " + PeriodDay + "й день";
            }
        }

        public static string SerializeParameters(int? period, IList<ReportParamInt> parameters)
        {
            return JsonConvert.SerializeObject(new SerializedParametersInfo { Period = period, Parameters = parameters.Select(i => Tuple.Create(i.InternalName, i.InstanceValue)).ToArray() });
        }

        public static SerializedParametersInfo DeserializeParameters(string p)
        {
            return JsonConvert.DeserializeObject<SerializedParametersInfo>(p);
        }

        public static Dictionary<int, string> Recurrencies = new Dictionary<int, string>
            {
                {0, "Ежемесячно"},
                {1, "Еженедельно"},
                {2, "Ежедневно"}
            };
    }

    public class SerializedParametersInfo
    {
        public int? Period { get; set; }
        public Tuple<string, object>[] Parameters { get; set; }
    }
}
