using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NW.Payment.Wrappers.EProPayment
{
    /// <summary>
    /// EPro payment processor
    /// </summary>
    public class Processor
    {

        /// <summary>
        /// Url that we submit the information to. eg: https://www.empcorp-lux.com/api/payment/direct
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
            ApiKey = "YmM5NjFiNjAzNmRhNjkxYzAwNTBiMTdkZTJiM2EwNmE1Mzc3ZmQ3NWQxMThhYjUxMzY4YWZjMDBiYTNhNTU2Zg==";
            //ApiKey = "ZTUzMDAyNDI0MTUxZjEwZGE4NzdiNjBhNzdiZThmYmFjZmY3NTM5ZDIzMjFhY2Y0MTZiMGUyNTEyZjA2YTc3OA=="; // for wall api
        }

        /// <summary>
        /// Process the payment.
        /// </summary>
        public string Process(DirectPayment directPayment)
        {
            try
            {
                //throw new Exception("Thrown exception intentionally by dev to process failover flow.");

                var httpHelper = new HttpHelper();
                if (String.IsNullOrEmpty(PaymentUrl))
                    //PaymentUrl = "https://www.empcorp-lux.com/api/payment/wall";
                    PaymentUrl = "https://www.empcorp-lux.com/api/payment/direct";

                var response = httpHelper.SendRequest(PaymentUrl, Serialize(directPayment), "EPRO-API-KEY:" + ApiKey);
                JsonResponse = response;
                var decodedResponse = Decode(response);
                DynamicResponse = decodedResponse;
                Encoded(directPayment);

               
                    if (decodedResponse.Code > 0)
                    {
                        return decodedResponse.Error;
                    }
                
              
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

            return "";
        }

        public string Status(string reference)
        {
            try
            {
                //throw new Exception("Thrown exception intentionally by dev to process failover flow.");

                var httpHelper = new HttpHelper();
                if (String.IsNullOrEmpty(PaymentUrl))
                    PaymentUrl = "https://www.empcorp-lux.com/api/status";

                var response = httpHelper.SendRequest(PaymentUrl, SerializeForStatus(reference), "EPRO-API-KEY:" + ApiKey);
                JsonResponse = response;
                var decodedResponse = Decode(response);
                DynamicResponse = decodedResponse;
                //Encoded(directPayment);


                if (decodedResponse.Code > 0)
                {
                    return decodedResponse.Error;
                }
                return decodedResponse.Result.Status;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

            return "";
        }

        private static dynamic Decode(string json)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(json);
            return obj;
        }

        public dynamic DynamicResponse { get; set; }
        public string JsonResponse { get; set; }
        public string JsonRequest { get; set; }
        private void Encoded(DirectPayment directPayment)
        {
            var obj = JsonConvert.SerializeObject(directPayment);
            JsonRequest = obj;
        }

      

        private static string Serialize(DirectPayment directPayment)
        {
            if (directPayment!=null)
            {
                var sr = new StringBuilder();
                sr.Append("Amount=" + directPayment.Amount);
                sr.Append("&Uid=" + directPayment.Uid);
                sr.Append("&Tid=" + directPayment.Tid);
                sr.Append("&Country=" + directPayment.Country);
                sr.Append("&BirthDate=" + directPayment.BirthDate);
                sr.Append("&Email=" + directPayment.Email);
                sr.Append("&Firstname=" + directPayment.Firstname);
                sr.Append("&Lastname=" + directPayment.Lastname);
                sr.Append("&CardNumber=" + directPayment.CardNumber);
                sr.Append("&CardMonth=" + directPayment.CardMonth);
                sr.Append("&CardYear=" + directPayment.CardYear);
                sr.Append("&CardCVV=" + directPayment.CardCvv);
                sr.Append("&ClientIp=" + directPayment.ClientIp);
                sr.Append("&OriginalCurrency=" + directPayment.Currency);
                sr.Append("&OriginalAmount=" + directPayment.ActualAmount*100);
                sr.Append("&ZipCode=" + directPayment.ZipCode);
                sr.Append("&3DS=yes"); // we force 3ds here! Our contract only allows 3ds payments. @cem
                sr.Append("&ReturnUrl=" + directPayment.ReturnUrl);

                return sr.ToString();
            } 
            return "";
        }

        private static string SerializeForStatus(string reference)
        {
           
                var sr = new StringBuilder();
                sr.Append("Reference=" + reference);
                

                return sr.ToString();
          
        }
    }
}