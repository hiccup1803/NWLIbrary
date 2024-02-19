using System;

namespace NW.Payment.Wrappers.ReyPay
{
    public class ReyPayResult<T>
    {
        public bool ResultStatus { get; set; }
        public int ResultCode { get; set; }
        public string ResultMessage { get; set; }

        public T ResultObject { get; set; }
    }
}