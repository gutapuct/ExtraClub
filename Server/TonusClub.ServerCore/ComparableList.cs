using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonusClub.ServerCore
{
    public class ComparableList<T> : List<T>, IComparable
        where T: IComparable
    {
        public override bool Equals(object obj)
        {
            if (!(obj is List<T>)) return false;
            var l2 = obj as List<T>;
            if (l2.Count != Count) return false;

            for (int i = 0; i < Count; i++)
            {
                if (!this[i].Equals(l2[i])) return false;
            }

            return true;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is List<T>)) return 0;
            var l2 = obj as List<T>;
            if (l2.Count != Count) Count.CompareTo(l2.Count);

            for (int i = 0; i < Count; i++)
            {
                if (this[i].Equals(l2[i])) continue;
                var cr = this[i].CompareTo(l2[i]);
                if (cr != 0) return cr;
            }
            return 0;
        }
    }
}
