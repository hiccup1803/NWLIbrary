using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NW.Helper.SMS
{
    public class PasifikSMSRequest
    {
        public static SMSResponse Send(string number, string text)
        {
            string json = "{{\"from\":\"08504300127\", \"to\":\"{0}\",\"text\":\"Dogrulama kodu: {1} , bu kodu siz talep etmediyseniz canli yardima baglanin.\"}}";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://oim.pasifiktelekom.com.tr/en/api/sms/submit/");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.ContentLength = byteArray.Length;
            request.Headers.Add("Authorization", "OTA4NTA0MzAwMTI2OmI1M2ZjQEY0ZyFw");
            //request.Headers.Add("Authorization", "OTA4NTA0MzAwMTI2OkhIa21McDQyS0xjYQ==");
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            string responseFromServer;
            try
            {
                using (Stream responseStream = request.GetResponse().GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    responseFromServer = reader.ReadToEnd();
                }
            }
            catch (WebException exception)
            {
                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                {
                    responseFromServer = reader.ReadToEnd();
                }
            }
            dataStream.Close();

            return new SMSResponse() { Success = responseFromServer.Contains("OK") };

        }
    }
}
