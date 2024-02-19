using System;
using System.Text;
using Newtonsoft.Json;


namespace NW.Payment.Wrappers.PayKasa
{
    /// <summary>
    /// Astro payment processor
    /// </summary>
    public class PayKasaProcessor
    {

        /// <summary>
        /// Url that we submit the information to. eg: https://test.api.paykasa.com/api/v3/vouchers/redemptions for test
        /// </summary>
        public string PaymentUrl { get; set; }

        /// <summary>
        /// Api key that is provided by the 3rd party payment gateway service.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Initialize processor with the default settings.
        /// </summary>
        /// <param name="apiKey"></param>
        public PayKasaProcessor(string apiKey)
        {
            ApiKey = apiKey;
        }

        public PayKasaProcessor()
        {
            ApiKey = "";
        }


        //private static dynamic Decode(string json)
        //{
        //    var obj = JsonConvert.DeserializeObject<dynamic>(json);
        //    return obj;
        //}

        public dynamic DynamicResponse { get; set; }
        public string JsonResponse { get; set; }
        public string JsonRequest { get; set; }
        private void Encoded(PayKasaRequest directPayment)
        {
            var obj = JsonConvert.SerializeObject(directPayment);
            JsonRequest = obj;
        }
     
    }
}