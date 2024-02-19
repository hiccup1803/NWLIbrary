using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Payment
{
    public class WithdrawResult
    {
        public bool Success { get; set; }
        public int ResponseCode { get; set; }

        public string ResponseDescription { get; set; }
        public Int64 TransactionId { get; set; }

        public Int64 Amount { get; set; }

        public string MerchantRef { get; set; }

        public string RedirectURL { get; set; }
    }
}
