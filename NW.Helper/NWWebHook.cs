using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NW.Helper
{
    public class NWWebHook
    {
        public static void PushBackOffice(string domain, string username, string action, string message)
        {
            Task.Run(() => MakeRequest(domain, username, action, message));
        }

        private static async void MakeRequest(string domain, string username, string action, string message)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.GetAsync(new Uri("https://" + domain + "/tr/webhookservice/push?username=" + username + "&message=" + message + "&action=" + action));
                }
                catch (Exception ex)
                {

                    var a = 1;
                }
            }
           
        }
    }
}
