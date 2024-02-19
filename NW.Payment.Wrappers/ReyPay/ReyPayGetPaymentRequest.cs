using System;

namespace NW.Payment.Wrappers.ReyPay
{
    public class ReyPayGetPaymentRequest
    {
        public string Wsuser { get; set; }
        public string Wspass { get; set; }
        public string Apikey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Card_Number1 { get; set; }
        public string Card_Number2 { get; set; }
        public string Card_Number3 { get; set; }
        public string Card_Number4 { get; set; }
        public string Card_Month { get; set; }
        public string Card_Year { get; set; }
        public string Card_Security { get; set; }
        public string Card_Name { get; set; }
        public string Card_Type { get; set; }
        public string Ip { get; set; }
        public double Price { get; set; }
    }
}