using System;

namespace NW.Payment.Wrappers.ReyPay
{
    public class ReyPayGetPaymentResponse
    {
        public string PaymentId { get; set; }
        public int TransTypeId { get; set; }
        public string Reference_Code { get; set; }
        public string Card_Number1 { get; set; }
        public string Card_Number2 { get; set; }
        public string Card_Number3 { get; set; }
        public string Card_Number4 { get; set; }
        public string Card_Month { get; set; }
        public string Card_Year { get; set; }
        public string Card_Security { get; set; }
        public string Card_Name { get; set; }
        public string Ip { get; set; }
        public string Price { get; set; }
        public string PaymentSmsId { get; set; }
        public string SmsStatus { get; set; }
        public string SmsContext { get; set; }
    }
}