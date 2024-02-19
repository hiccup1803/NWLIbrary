using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Payment.Wrappers.BP
{
    public class BPCommercialBankRequest
    {
        [JsonProperty("apiKey")]
        public string MerchantKey { get; set; }
        [JsonProperty("referenceNo")]
        public string MerchantRef { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("amount")]
        public Int64 Amount { get; set; }
        [JsonProperty("billingFirstName")]
        public string Firstname { get; set; }
        [JsonProperty("billingLastName")]
        public string Lastname { get; set; }
        [JsonProperty("billingCountry")]
        public string Country { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("customerIp")]
        public string IP { get; set; }
        [JsonProperty("customerUserAgent")]
        public string CustomerUserAgent { get; set; }
        [JsonProperty("returnUrl")]
        public string ReturnUrl { get; set; }
        [JsonProperty("withdrawId")]
        public int WithdrawId { get; set; }
    }
}
