using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Payment
{
    public class CryptoExchangePrice
    {
        public DateTime date { get; set; }
        public Usd USD { get; set; }
        public Gbp GBP { get; set; }
        public Eur EUR { get; set; }
        public Try TRY { get; set; }
        public Jpy JPY { get; set; }

        public class Usd
        {
            public float bid { get; set; }
            public float ask { get; set; }
        }

        public class Gbp
        {
            public float bid { get; set; }
            public float ask { get; set; }
        }

        public class Eur
        {
            public float bid { get; set; }
            public float ask { get; set; }
        }

        public class Try
        {
            public float bid { get; set; }
            public float ask { get; set; }
        }

        public class Jpy
        {
            public float bid { get; set; }
            public float ask { get; set; }
        }

    }
}
