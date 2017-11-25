using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewtonicServer.Server
{
    class TextInfo
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }

        public int Position1 { get; set; }
        public int Position2 { get; set; }

        public DateTime? Timeout { get; set; }

        public TextInfo(string line1, string line2, int timeout)
        {
            Line1 = line1;
            Line2 = line2;

            if (Line1.Length > 16) Line1 = Line1 + "   ";
            if (Line2.Length > 16) Line2 = Line2 + "   ";

            Position1 = 0;
            Position2 = 0;

            if (timeout > 0)
            {
                Timeout = DateTime.Now.AddSeconds(timeout);
            }
        }

        public void Delta()
        {
            if (DateTime.Now > Timeout) return;
            if (Line1.Length > 16)
            {
                Position1++;
                if (Position1 > Line1.Length) Position1 = 0;
            }
            if (Line2.Length > 16)
            {
                Position2++;
                if (Position2 > Line2.Length) Position2 = 0;
            }
        }

        public string DeltedLine1
        {
            get
            {
                if (DateTime.Now > Timeout) return "Добро пожаловать";

                if (Position1 == 0) return Line1.Length > 16 ? Line1.Substring(0,16) : Line1;
                return (Line1 + Line1).Substring(Position1, Math.Min((Line1 + Line1).Length - Position1, 16));
            }
        }

        public string DeltedLine2
        {
            get
            {
                if (DateTime.Now > Timeout) return "в ТОНУС-КЛУБ!";

                if (Position2 == 0) return Line2.Length > 16 ? Line2.Substring(0, 16) : Line2;
                return (Line2 + Line2).Substring(Position2, Math.Min((Line2 + Line2).Length - Position2, 16));
            }
        }
    }
}
