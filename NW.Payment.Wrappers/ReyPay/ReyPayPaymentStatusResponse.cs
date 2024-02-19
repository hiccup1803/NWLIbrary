using System;

namespace NW.Payment.Wrappers.ReyPay
{
    public class ReyPayPaymentStatusResponse
    {
        public string PaymentStatusId { get; set; }
        public string Reference_Code { get; set; }
        public string Status_3D { get; set; }
        public string OrderReference_3D { get; set; }
        public string ExceptionMessage_3D { get; set; }
        public string PaymentGuId_3D { get; set; }
        public string InternalMessage { get; set; }
        public string AutorozationCode { get; set; }
        public string HostKey { get; set; }
        public string BankErrorCode { get; set; }
        public string BankMessage { get; set; }
        public string IsSuccess { get; set; }
        public string IsOkey { get; set; }
    }
}