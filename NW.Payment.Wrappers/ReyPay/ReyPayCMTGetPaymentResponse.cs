using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Payment.Wrappers.ReyPay
{
    public class ReyPayCMTGetPaymentResponse
    {
        public string PaymentId { get; set; }
        public string Reference_Code { get; set; }
        public int ProcessId { get; set; }
        public string PaymentUrl { get; set; }
    }
}
