using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Helper.SMS
{
    public interface ISMSRequest
    {
        string User { get; set; }
        string Password { get; set; }
        string ApiId { get; set; }
        string To { get; set; }
        string From { get; set; }
        string Message { get; set; }

        SMSResponse Send(string username, int memberId);
    }

    public interface ISMSResponse
    {
        bool Success { get; set; }
        string Message { get; set; }
    }

    public class SMSResponse : ISMSResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class SMSRequest : ISMSRequest
    {
        public string User { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Will be set if left empty.
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// Mobile numbers. Eg: 447788446834, can be comma seperated.
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Sender Id
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Message to send. 
        /// </summary>
        public string Message { get; set; }

        public SMSResponse Send(string username, int memberId)
        {
            //if (string.IsNullOrEmpty(ApiId))
            //    ApiId = "3560923";// Smart api id "3488934";

            //var smsRequester = new HttpHelper();

            //if (!string.IsNullOrEmpty(From))
            //{
            //    From = "&from=" + From;
            //}
            ////var postData = "user=navysms&password=cdWSXNdLMFQYSD&api_id=" + ApiId + "&from=" + From + "&to=" + To + "&text=" +Message;
            //var postData = "user=navysms&password=cdWSXNdLMFQYSD&api_id=" + ApiId + From +"&to=" + To + "&text=" + Message;

            //var response =smsRequester.SendRequest(string.Empty, postData,"GET");

            //return response;


            StringBuilder sb = new StringBuilder();
            sb.Append("{\"SmsProviderId\":4, \"CompanyId\":6, \"ScheduleDate\":\"").Append(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm")).Append("\", \"Message\":\"")
                .Append(Message.Trim()).Append("\", \"CreatedBy\":\"WEB-VERIFICATIONPAGE\", \"Number\":\"" + To + "\", \"Username\":\"" + username + "\", \"MemberId\":\"" + memberId + "\"}");



            string key = "jJm9fGRPxcRPWxfFdfx2M9xSqrs9LR4ZgHCJ3LncnRGncZHagNujKCbGmxrn7Qvr5vvd6bkntPQSQkknSUEp9fMBV4VNff2XkVFqpz8cBrsEmrN7ugMnRG5nkTpNWZCwgjkd5JrPpXkbEZ79XYyHWrETgDP9bgTHV2pTKBRTFmphx26XerC8VwYJH6QjyENRQeFuQVndjXmyZRkE6EmQmUW4fK7wYpfNY9dJzMRc4pLCAXMecccNkWEbBP6bhpH6yzzXpUNLQWdUkBvvB6BghJDX2jLK75RGz3r75GytfTEp6tFZuUHNqkRgHbrNv8fcrQUtfKAFUYSpvNfMnQAe8rFhqWuXa2bZJqJdwEcwyga99CFGf3TEEsfJVf8rDVSXbMMfmpx36wBrtA6NmKn9svYYgEuL4qtt4kPnmucdSsMtWcnEz5aE7ayptQmrhFqk7QZk7eQgP567eXDWkxNMVxNuWJTP33qHwGrhhrty8YLRwLCLbS3m";
            NW.Helper.HttpHelper.PostRequest("http://k1.nwservmodule.com/marketing/sms?c=" + Security.SecurityHelper.MD5Encryption(key + sb.ToString()), sb.ToString(), "application/json");

            return new SMSResponse() { Success = true };
        }


    }
}
