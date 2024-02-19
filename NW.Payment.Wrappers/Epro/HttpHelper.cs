using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NW.Payment.Wrappers.EProPayment
{
    internal class HttpHelper
    {
        public string SendRequest(string url, string postData, string header)
        {
            // Create web request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 30000;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Headers.Add(header);

            var encoding = new UTF8Encoding();
            byte[] postdata = encoding.GetBytes(postData);
            var writer = httpWebRequest.GetRequestStream();
            writer.Write(postdata, 0, postdata.Length);
            writer.Close();

            // Make the request and get the response
            var response = (HttpWebResponse)httpWebRequest.GetResponse();
            var dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            string responseFromServer = "";
            using (var reader = new StreamReader(dataStream))
            {
                // Read the content.
                responseFromServer = reader.ReadToEnd();
            }
            return responseFromServer;
        }
    }
}
