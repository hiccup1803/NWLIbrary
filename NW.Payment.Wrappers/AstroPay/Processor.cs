using System;
using System.Text;
using Newtonsoft.Json;


namespace NW.Payment.Wrappers.AstroPay
{
    /// <summary>
    /// Astro payment processor
    /// </summary>
    public class Processor
    {

        /// <summary>
        /// Url that we submit the information to. eg: https://www.astropaycard.com/verif/validator.php
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
        public Processor(string apiKey)
        {
            ApiKey = apiKey;
        }

        public Processor()
        {
            ApiKey = "";
        }

        /// <summary>
        /// Process the payment.
        /// </summary>
        public dynamic Process(AstroRequest directPayment, string astroLogin, string astroTranKey, string astroVersion, string astroURL)
        {
            try
            {
                //throw new Exception("Thrown exception intentionally by dev to process failover flow.");

                var httpHelper = new HttpHelper();
                if (String.IsNullOrEmpty(PaymentUrl))
                    PaymentUrl = astroURL ; 

                var response = httpHelper.SendRequest(PaymentUrl, Serialize(directPayment, astroLogin, astroTranKey, astroVersion), string.Empty);
                return response;
            }
            catch (Exception ex)
            {
                return new { code = 0, reason_text = ex.Message };
            }

        }

        //private static dynamic Decode(string json)
        //{
        //    var obj = JsonConvert.DeserializeObject<dynamic>(json);
        //    return obj;
        //}

        public dynamic DynamicResponse { get; set; }
        public string JsonResponse { get; set; }
        public string JsonRequest { get; set; }
        private void Encoded(AstroRequest directPayment)
        {
            var obj = JsonConvert.SerializeObject(directPayment);
            JsonRequest = obj;
        }

        private dynamic Serialize(AstroRequest directPayment, string astroLogin, string astroTranKey, string astroVersion)
        {

            //if (directPayment != null)
            //{
            //    return new
            //    {
            //        x_login = astroLogin,
            //        x_tran_key = astroTranKey,
            //        x_version = astroVersion,
            //        x_type = "AUTH_CAPTURE",
            //        x_test_request = directPayment.TestRequest,
            //        x_card_num = directPayment.CardNumber,
            //        x_card_code = directPayment.CardCvv,
            //        x_exp_date = directPayment.CardExpDate,
            //        x_amount = directPayment.Amount,
            //        x_unique_id = directPayment.Uid,
            //        x_invoice_num = directPayment.InvoiceNum,
            //        x_response_format = "json"
            //    };
            //}
            //return "";

            if (directPayment != null)
            {
                var sr = new StringBuilder();
                sr.Append("x_login=" + astroLogin);
                sr.Append("&x_tran_key=" + astroTranKey);
                sr.Append("&x_version=" + astroVersion);
                sr.Append("&x_type=" + "AUTH_CAPTURE");
                sr.Append("&x_test_request=" + directPayment.TestRequest);
                sr.Append("&x_card_num=" + directPayment.CardNumber);
                sr.Append("&x_exp_date=" + directPayment.CardExpDate);
                sr.Append("&x_card_code=" + directPayment.CardCvv.ToString("0000"));
                sr.Append("&x_amount=" + directPayment.Amount);
                sr.Append("&x_unique_id=" + directPayment.Uid);
                sr.Append("&x_currency=" + directPayment.Currency);
                //sr.Append("&x_currency=USD");
                sr.Append("&x_invoice_num=" + directPayment.InvoiceNum);
                sr.Append("&x_response_format=" + "json");

                return sr.ToString();
            }
            return "";
        }
    }
}