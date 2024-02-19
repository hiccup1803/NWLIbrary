using System;

namespace NW.Payment.Wrappers.ReyPay
{
    public class ReyPaySmsStatusResponse
    {
        public string PaymentSmsId { get; set; }
        public string Reference_Code { get; set; }
        public string Status { get; set; }
        public string Context { get; set; }
    }
}