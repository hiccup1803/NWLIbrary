using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Logging;
using Newtonsoft.Json;
using NW.Service;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace NW.Services
{
    public class HttpServiceHelper
    {
        public const string JSON_CONTENT_TYPE = "application/json";
        protected static void FinishWebRequest(IAsyncResult result)
        {
            httpWebRequest.EndGetResponse(result);
        }
        private static HttpWebRequest httpWebRequest;

        public static T PostXMLRequest<T>(string url, string xml) where T : class
        {
            dynamic result;
            string content = "";
            Logging.Logger Logger = new Logger("HTTP");
            // Make the request and get the response
            try
            {
                // Create web request
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 60000;
                httpWebRequest.ContentType = "application/xml";
                httpWebRequest.Proxy = null; // this will speed up the process as skipping the proxy check.
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                System.Net.ServicePointManager.Expect100Continue = false;

                var encoding = new UTF8Encoding();
                byte[] postdata = encoding.GetBytes(xml);
                using (var writer = httpWebRequest.GetRequestStream())
                {
                    writer.Write(postdata, 0, postdata.Length);
                    writer.Close();
                }
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

                //response = (HttpWebResponse)
                //httpWebRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
                
                using (var stream = new StreamReader(response.GetResponseStream()))
                {
                    content = stream.ReadToEnd();
                }
                Logger.Debug("Url: " + url + " ,Request xml: " + xml + " \r\nResponse Content:" + content);
                //result = JsonConvert.DeserializeObject(content);

                result = ParseHelpers.ParseXML<T>(content);

            }
            catch (Exception ex)
            {
                Logger.Fatal("Content:" + content + ", Error: " + ex.Message,ex);
                result = new { status = 0, code=Guid.NewGuid().ToString("N"), errorMessage = "Connection Error! ex:" + ex.Message };
            }

            return result;
        }

        public static dynamic PostRequest(string url, string postJson, string contentType, params NameValueCollection[] headerCollection)
        {
            dynamic result;
            // Make the request and get the response
            try
            {

                // Create web request

                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 100000;
                httpWebRequest.ContentType = contentType;
                httpWebRequest.Proxy = null; // this will speed up the process as skipping the proxy check.

                System.Net.ServicePointManager.Expect100Continue = false;

                foreach (NameValueCollection nvc in headerCollection)
                {
                    httpWebRequest.Headers.Add(nvc);
                }


                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;




                var encoding = new UTF8Encoding();
                byte[] postdata = encoding.GetBytes(postJson);
                using (var writer = httpWebRequest.GetRequestStream())
                {
                    writer.Write(postdata, 0, postdata.Length);
                    writer.Close();
                }
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

                //response = (HttpWebResponse)
                //httpWebRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
                var content = "";
                using (var stream = new StreamReader(response.GetResponseStream()))
                {
                    content = stream.ReadToEnd();
                }
                result = JsonConvert.DeserializeObject(content);
            }
            catch (Exception ex)
            {
                result = new { status = 0, errorMessage = "Connection Error! ex:" + ex.Message };
            }

            return result;
        }
        public static PostRequestModel PostJsonRequest(string url, string postJson, string contentType, params NameValueCollection[] headerCollection)
        {
            PostRequestModel returnModel = new PostRequestModel();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;


                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 100000;
                httpWebRequest.ContentType = contentType;
                httpWebRequest.Proxy = null; // this will speed up the process as skipping the proxy check.

                System.Net.ServicePointManager.Expect100Continue = false;

                foreach (NameValueCollection nvc in headerCollection)
                {
                    httpWebRequest.Headers.Add(nvc);
                }

                var encoding = new UTF8Encoding();
                byte[] postdata = encoding.GetBytes(postJson);
                using (var writer = httpWebRequest.GetRequestStream())
                {
                    writer.Write(postdata, 0, postdata.Length);
                    writer.Close();
                }
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

                //response = (HttpWebResponse)
                //httpWebRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
                var content = "";
                using (var stream = new StreamReader(response.GetResponseStream()))
                {
                    content = stream.ReadToEnd();
                }
                returnModel.Obj = (JObject)JsonConvert.DeserializeObject(content);
                returnModel.Succes = true;
            }
            catch (Exception ex)
            {
                JObject obj = new JObject();
                obj["message"] = ex.Message;
                returnModel.Obj = obj;
            }

            return returnModel;
        }
        public static dynamic PostJsonRequest(string url, string postJson, params NameValueCollection[] headerCollection)
        {
            return PostRequest(url, postJson, "application/json", headerCollection);
        }
        public static dynamic GetJsonRequest(string url)
        {
            dynamic result;
            // Make the request and get the response
            try
            {

                // Create web request

                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 100000;
                httpWebRequest.Proxy = null; // this will speed up the process as skipping the proxy check.

                System.Net.ServicePointManager.Expect100Continue = false;
                
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

                //response = (HttpWebResponse)
                //httpWebRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
                var content = "";
                using (var stream = new StreamReader(response.GetResponseStream()))
                {
                    content = stream.ReadToEnd();
                }
                result = JsonConvert.DeserializeObject(content);
            }
            catch (Exception ex)
            {
                result = new { status = 0, errorMessage = "Connection Error! ex:" + ex.Message };
            }

            return result;
        }


        public static T GetJsonRequest<T>(string url)
        {
            T result;
            try
            {
                // Create web request
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 100000;
                httpWebRequest.Proxy = null; // this will speed up the process as skipping the proxy check.

                System.Net.ServicePointManager.Expect100Continue = false;

                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                var content = "";
                using (var stream = new StreamReader(response.GetResponseStream()))
                {
                    content = stream.ReadToEnd();
                }
                result = JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                result = JsonConvert.DeserializeObject<T>("{ status = 0, errorMessage = \"Connection Error! ex:\" + " + ex.Message + "}");
            }

            return result;
        }

        

    }
}