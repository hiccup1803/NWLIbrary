using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace NW.Payment.Wrappers.AstroPay
{
    public class HttpHelper
    {
        public dynamic SendPostRequest(string url, string postData)
        {
            var timeout = 100000; // 100 sec for long running queries 

            // Create web request
            HttpWebRequest httpWebRequest;
            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = timeout;
            //httpWebRequest.ContentType = "application/json";
            //httpWebRequest.Headers["Accept"] = "application/json";
            //httpWebRequest.Headers["Accept-Encoding"] = "gzip, deflate";

            System.Net.ServicePointManager.Expect100Continue = false;
            
            var encoding = new UTF8Encoding();
            byte[] postdata = encoding.GetBytes(postData);
            using (var writer = httpWebRequest.GetRequestStream())
            {
                writer.Write(postdata, 0, postdata.Length);
                writer.Close();
            }
           
            // Make the request and get the response
            HttpWebResponse response = null;
            //response = (HttpWebResponse)httpWebRequest.GetResponse();
            response = (HttpWebResponse)httpWebRequest.GetResponse();

            var message = "";
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                message = reader.ReadToEnd();
            }
            return message;
        }


        public dynamic SendRequest(string url, dynamic postData, string header)
        {

            // Create web request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "?" + postData);
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 30000;
            //httpWebRequest.ContentType = "application/json";
            httpWebRequest.ContentType = "text/html; charset=UTF-8";
            
            //httpWebRequest.Headers.Add(header);

            //var strpostdata = JsonConvert.SerializeObject(postData);
            var encoding = new UTF8Encoding();
            
            //byte[] postdata = encoding.GetBytes(strpostdata);
            
            //var writer = httpWebRequest.GetRequestStream();
            //writer.Write(postdata, 0, postdata.Length);
            //writer.Close();

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
            var jsonResult = JsonConvert.DeserializeObject(responseFromServer);
            return jsonResult;
        }
    }
}
