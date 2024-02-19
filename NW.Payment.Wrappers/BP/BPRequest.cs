using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Payment.Wrappers.BP
{
    public class BPRequest
    {
        [JsonProperty("defaultPaymentMethod")]
        public string PaymentMethod { get; set; }
        [JsonProperty("apiKey")]
        public string MerchantKey { get; set; }
        [JsonProperty("referenceNo")]
        public string MerchantRef { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonIgnore]
        public decimal Amount { get; set; }
        [JsonProperty("amount")]
        public string AmountString
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture,
                                 "{0:0.00}", Amount);
            }
        }
        [JsonProperty("firstName")]
        public string Firstname { get; set; }
        [JsonProperty("lastName")]
        public string Lastname { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("dateOfBirth")]
        public string DateOfBirth { get; set; }
        [JsonProperty("postCode")]
        public string PostalCode { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("cardNumber")]
        public string CardNo { get; set; }
        [JsonProperty("expDate")]
        public string ExpiryDate { get; set; }
        [JsonProperty("pin")]
        public string CVV { get; set; }
        [JsonProperty("successRedirectUrl")]
        public string SuccessURL { get; set; }
        [JsonProperty("failRedirectUrl")]
        public string FailURL { get; set; }
        [JsonProperty("is3d")]
        public bool Is3d { get; set; }
        [JsonProperty("only3d")]
        public bool Only3d { get; set; }
    }
}
