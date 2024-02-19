using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NW.Payment.Wrappers.Citigate
{
    public class TransactionResponse
    {
        public Int64 TransactionID { get; set; }
        public string MerchantRef { get; set; }
        public int TransTypeID { get; set; }
        public string Currency { get; set; }
        public Int64 Amount { get; set; }
        public string BusinessCase { get; set; }
        public string Descriptor { get; set; }
        public string Bank { get; set; }

        public int ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string BankCode { get; set; }
        public string BankDescription { get; set; }

        public string RedirectURL { get; set; }

    }
}
