using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;



namespace NW.Helper.SMS
{
    public class HttpHelper
    {

        public HttpHelper()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public SMSResponse SendRequest(string function, string data, string method = "POST")
        {
            var log = new Logging.Logger("SMS");
            log.Fatal("Clickatell Sending Request-Func:" + function + "-data:" + data);

            var _response = new SMSResponse{Success = false, Message = "Unknown error!"};

            var timeout = 100000; // 100 sec for long running queries 

            // Create web request
            HttpWebRequest httpWebRequest;
            httpWebRequest = (HttpWebRequest)WebRequest.Create("http://api.clickatell.com/http/sendmsg?" + data);
            httpWebRequest.Method = method;
            httpWebRequest.Timeout = timeout;
            //httpWebRequest.ContentType = "application/json";
            //httpWebRequest.Headers["Accept"] = "application/json";
            //httpWebRequest.Headers["Accept-Encoding"] = "gzip, deflate";

            System.Net.ServicePointManager.Expect100Continue = false;

            if (method=="POST")
            {
                var encoding = new UTF8Encoding();
                byte[] postdata = encoding.GetBytes(data);
                using (var writer = httpWebRequest.GetRequestStream())
                {
                    writer.Write(postdata, 0, postdata.Length);
                    writer.Close();
                }
            }
            // Make the request and get the response
            HttpWebResponse response = null;
            //response = (HttpWebResponse)httpWebRequest.GetResponse();
            try
            {
                var requestSuccessful = false;
                var tryCount = 3;
                var errorMessage = "";

                for (var i = 0; i < tryCount; i++)
                {
                    try
                    {
                        if (!requestSuccessful)
                        {
                            response = (HttpWebResponse)httpWebRequest.GetResponse();
                            requestSuccessful = true;
                            var message = "";
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                message = reader.ReadToEnd();
                            }

                            log.Fatal("HttpSMS: " + message);

                            if (message.Contains("ID:"))
                            {
                                // successfully processed.
                                _response.Success = true;
                                _response.Message = "";
                            }
                            else
                            {
                                _response.Message = message;
                            }

                            break; // exit loop.
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                        // will try again 3 times. tryCount.
                    }
                }

                if (!requestSuccessful)
                {
                    throw new Exception(errorMessage);
                }

            }
            catch (Exception ex)
            {
                log.Fatal("HttpHelper SendRequest- func:" + function + "-Occured error:", ex);
                //throw new Exception("Unable to reach clickatell. Err:" + ex.Message);
                _response.Message = ex.Message;
                _response.Success = false;
            }
            return _response;
        }
    }
}
