using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Payment
{
    public class JetonVoucherAdditinalInfo
    {
        public long Amount { get; set; }
        public string Currency { get; set; }
        public long AmountJetonCurrency { get; set; }
    }
}
