using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Payment
{
    public class CryptoAdditionalInfo
    {
        public long Amount { get; set; }
        public string Currency { get; set; }
        public string CryptoWalletAddress { get; set; }
        public decimal Rate { get; set; }
    }
}
