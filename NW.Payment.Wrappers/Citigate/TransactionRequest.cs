using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NW.Payment.Wrappers.Citigate
{
    public class TransactionRequest
    {
        public int PaymentTypeId { get; set; }
        public int TransTypeId { get; set; }
        public string MerchantName { get; set; }
        public string MerchantPassword { get; set; }
        public string MerchantRef { get; set; }
        public string Currency { get; set; }
        public Int64 Amount { get; set; }
        public string Brand { get; set; }
        public string CardholderName { get; set; }
        public string CardNo { get; set; }
        public int ExpiryYear { get; set; }
        public int ExpiryMonth { get; set; }
        public string CVV { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string StreetLine1 { get; set; }
        public string StreetLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string StateProvince { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string DateOfBirth { get; set; }
        public string UserIP { get; set; }
        public string SuccessURL { get; set; }
        public string FailURL { get; set; }
        public string CallbackURL { get; set; }
    }
}