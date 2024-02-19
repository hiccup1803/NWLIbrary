using Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NW.Helper
{
    public class HttpHelperResponseModel
    {
        public string Result { get; set; }
        public Dictionary<string, string> Header { get; set; }
    }

    public class HttpHelper
    {
        public static JObject GetJsonRequest(string url)
        {
            dynamic result;
            HttpWebResponse response = null;
            // Make the request and get the response
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 100000;
                httpWebRequest.Proxy = null; // this will speed up the process as skipping the proxy check.

                System.Net.ServicePointManager.Expect100Continue = false;
				
				if (url.ToLower().StartsWith("https"))
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                response = (HttpWebResponse)httpWebRequest.GetResponse();

                //response = (HttpWebResponse)
                //httpWebRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
                //new Logger("BO").Fatal("URL: " + url);
                var content = "";
                using (var stream = new StreamReader(response.GetResponseStream()))
                {
                    content = stream.ReadToEnd();
                }
                
                //new Logger("BO").Fatal("RESULT: " + content);
                result = (JObject)JsonConvert.DeserializeObject(content);
            }
            //catch (WebException exception)
            //{
            //    using (var reader = new StreamReader(exception.Response.GetResponseStream()))
            //    {
            //        result = (JObject)JsonConvert.DeserializeObject(reader.ReadToEnd());
            //    }
            //}
            catch (Exception ex)
            {
                result = new { status = 0, errorMessage = "Connection Error! ex:" + ex.Message };
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response.Dispose();
                }
            }
            

            return result;
        }
        public static string PostRequest(string url, string body, string contentType = "text/xml", params NameValueCollection[] headerCollection)
        {
            HttpHelperResponseModel result = MakeRequestResponseWithHeader(url, body, "POST", contentType, headerCollection);
            return result.Result;
        }

        public static HttpHelperResponseModel MakeRequestResponseWithHeader(string url, string body, string method = "POST", string contentType = "text/xml", params NameValueCollection[] headerCollection)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpHelperResponseModel resultModel = new HttpHelperResponseModel();
            HttpWebResponse response = null;
            // Make the request and get the response
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = method;
                httpWebRequest.ContentType = contentType;
                httpWebRequest.Timeout = 500000;
                //httpWebRequest.Proxy = null; // this will speed up the process as skipping the proxy check.



                foreach (NameValueCollection nvc in headerCollection)
                {
                    httpWebRequest.Headers.Add(nvc);
                }


                System.Net.ServicePointManager.Expect100Continue = false;

                if(method != "GET")
                {
                    //request stream
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(body);
                    }
                }

                response = (HttpWebResponse)httpWebRequest.GetResponse();

                //response = (HttpWebResponse)
                //httpWebRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
                using (var stream = new StreamReader(response.GetResponseStream()))
                {
                    resultModel.Result = stream.ReadToEnd();
                    resultModel.Header = new Dictionary<string, string>();

                    foreach(string key in response.Headers.AllKeys)
                    {
                        resultModel.Header.Add(key, response.Headers[key]);
                    }

                    new Logger("BO").Fatal("URL: " + url + " RESULT: " + resultModel.Result);
                }
            }
            catch (WebException exception)
            {
                new Logger("BO").Fatal("WebException URL: " + url);
                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                {
                    resultModel.Result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                new Logger("BO").Fatal("URL: " + url + " EX: " + ex.Message);
                resultModel.Result = ex.Message;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response.Dispose();
                }
            }

            return resultModel;
        }

        public static string ReadHeaderPostRequest(string url, string body, string headerKey, string contentType = "text/xml", params NameValueCollection[] headerCollection)
        {
            string headerValue = string.Empty;

            HttpHelperResponseModel result = MakeRequestResponseWithHeader(url, body, "POST", contentType, headerCollection);
            if (result.Header.ContainsKey(headerKey))
            {
                headerValue = result.Header[headerKey];
            }

            return headerValue;
        }
    }
}
