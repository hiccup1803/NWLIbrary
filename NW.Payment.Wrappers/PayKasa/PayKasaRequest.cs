using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NW.Payment.Wrappers.PayKasa
{

    /// <summary>
    /// 
    /// </summary>
    public class PayKasaRequest
    {
        
        ///<summary>
        /// Login for merchant
        /// </summary>
        [JsonProperty(PropertyName = "api_token")]
        public string ApiToken { get; set; }

        ///<summary>
        /// Code
        /// </summary>
        [JsonProperty(PropertyName = "voucher_code")]
        public string VoucherCode { get; set; }

        /// <summary>
        /// Unique id in paykasa transaction table 
        /// </summary>
        [JsonProperty(PropertyName = "customer_order_id")]
        public string CustomerOrderId { get; set; }

        /// <summary>
        /// Player id
        /// </summary>
        [JsonProperty(PropertyName = "customer_id")]
        public string CustomerId { get; set; }
      

        /// <summary>
        /// Amount of the transaction. MUST BE IN EUR
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        /// <summary>
        /// IP of the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "customer_ip_addr")]
        public string IPAddress { get; set; }

        /// <summary>
        /// Email of the customer.
        /// </summary>
        [JsonProperty(PropertyName = "customer_email")]
        public string CustomerEmail { get; set; }

        /// <summary>
        /// Country of the customer.
        /// </summary>
        [JsonProperty(PropertyName = "customer_country")]
        public string CustomerCountry { get; set; }
    }
}