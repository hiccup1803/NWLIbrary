using System;

namespace NW.Payment.Wrappers.ReyPay
{
    public class ReyPayNewCustomerResponse
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Mail { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Date_Day { get; set; }
        public string Date_Month { get; set; }
        public string Date_Year { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Adress { get; set; }
        public string Phone { get; set; }
        public string Identity { get; set; }
    }
}