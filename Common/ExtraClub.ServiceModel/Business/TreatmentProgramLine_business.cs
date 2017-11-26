using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel
{
    partial class TreatmentProgramLine : IComparable
    {
        public int CompareTo(object obj)
        {
            if (!(obj is TreatmentProgramLine)) return 0;
            return Position.CompareTo((obj as TreatmentProgramLine).Position);
        }
    }
}
