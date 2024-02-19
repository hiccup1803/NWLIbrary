using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Logging;
using NW.Helper.Email.Entities;
using RestSharp;

namespace NW.Helper.Email
{
    public class SmtpApi
    {

        private string BaseUrl { get { return "http://transsendapi.cloudapp.net/transsend/api/"; } }
        //private string BaseUrl { get { return "http://api.transsendapi.net/transsend/api/"; } }
        private string AddCampaignUrl { get { return "campaigns/add"; } }
        private string AddTemplateUrl { get { return "templates/add"; } }
        private string SendUrl { get { return "send"; } }


        public string ApiKey { get; set; }
        private Logger Logger { get; set; }

        public SmtpApi(string apiKey)
        {
            ApiKey = apiKey;
            Logger=new Logger("SMTPAPI");
        }

        public string AddCampaign(string campaignName)
        {
            var client = new RestClient(BaseUrl);
            // client.Authenticator = new HttpBasicAuthenticator(username, password);
            
            var request = new RestRequest(AddCampaignUrl, Method.GET);
            request.AddParameter("ApiKey", ApiKey);
            request.AddParameter("CampaignName", campaignName);

            // execute the request
            var response = client.Execute(request);
            //var content = response.Content; // raw content as string

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Logger.Fatal("Response OK:" + response.Content);

                //var result = JsonConvert.DeserializeObject<dynamic>(response.Content);
                //result.BonusBalance = response.Data["BonusBalance"];
            }
            else
            {
                Logger.Fatal("Request failed:" + response.Content);
            }

            return response.Content;
        }

        public string AddTemplate(string templateName, string html, string subject, string from , string fromName)
        {
            var client = new RestClient(BaseUrl);
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest(AddTemplateUrl, Method.POST) { RequestFormat = DataFormat.Json };
            var e = new { ApiKey = ApiKey, From = from, FromName = fromName, TemplateName = templateName, Html = html, Subject = subject };
            string req = request.JsonSerializer.Serialize(e);

            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", req, ParameterType.RequestBody);

            //var request = new RestRequest(AddTemplateUrl, Method.GET);
            //request.AddParameter("ApiKey", ApiKey);
            //request.AddParameter("TemplateName", templateName);
            //request.AddParameter("Html", html);
            //request.AddParameter("Subject", subject);
            //request.AddParameter("From", from);
            //request.AddParameter("FromName", fromName);

            // execute the request
            var response = client.Execute(request);
            //var content = response.Content; // raw content as string

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Logger.Fatal("Response OK:" + response.Content);

                //var result = JsonConvert.DeserializeObject<dynamic>(response.Content);
                //result.BonusBalance = response.Data["BonusBalance"];
            }
            else
            {
                Logger.Fatal("Request failed:" + response.Content);
            }

            return response.Content;
        }
         

        public string SendToQueue(string bodyHtml,string subject, string from, string fromName, List<string> recipients)
        {
            var client = new RestClient(BaseUrl);
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest(SendUrl, Method.POST) {RequestFormat = DataFormat.Json};

            List<Recipient> _recipients = recipients.Select(rec => new Recipient {ToAddress = rec}).ToList();

            var e = new EmailQueue { ApiKey = ApiKey, From = from, FromName = fromName, Recipients = _recipients, BodyHtml = bodyHtml, Subject = subject };
            string req = request.JsonSerializer.Serialize(e);

            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json",req, ParameterType.RequestBody);

            // execute the request
            var response = client.Execute(request);
            //var content = response.Content; // raw content as string

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Logger.Fatal("Response OK:" + response.Content);

                //var result = JsonConvert.DeserializeObject<dynamic>(response.Content);
                //result.BonusBalance = response.Data["BonusBalance"];
            }
            else
            {
                Logger.Fatal("Request failed:" + response.Content);
            }

            return response.Content;
        }
    }
}
