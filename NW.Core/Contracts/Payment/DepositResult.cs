using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Payment
{


    public class DepositResult
    {
        public bool Success { get; set; }
        public int ResponseCode { get; set; }

        public string ResponseDescription { get; set; }
        public string TransactionId { get; set; }

        public long Amount { get; set; }

        public string MerchantRef { get; set; }

        public string RedirectURL { get; set; }
    }
}
