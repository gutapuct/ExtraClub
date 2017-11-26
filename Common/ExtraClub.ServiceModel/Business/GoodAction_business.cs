using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class GoodAction: IInitable
    {
        [DataMember]
        public string GoodsList { get; private set; }

        [DataMember]
        public Dictionary<Guid, int> SerializedGoodActions { get; private set; }

        public void Init()
        {
            var sb = new StringBuilder();

            foreach (var gl in GoodActionLines)
            {
                if (sb.Length > 0) sb.Append(" ;");
                sb.Append(gl.Good.Name + " (" + gl.Amount + " " + gl.Good.UnitType.Name + ")");
            }

            GoodsList = sb.ToString();

            SerializedGoodActions = GoodActionLines.ToDictionary(i => i.GoodId, j => j.Amount);
        }
    }
}
