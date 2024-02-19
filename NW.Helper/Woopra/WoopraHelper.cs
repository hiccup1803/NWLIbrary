using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net.Http;

namespace NW.Helper
{
    public class WoopraHelper
    {
        public static void TriggerAction(string action, int memberId, string username, string affCode)
        {
            Task.Run(() => MakeCustomRequest(action, memberId, username, affCode));
        }

        public static void TriggerDepositAction(int memberId, string username, decimal amount, string provider)
        {
            Task.Run(() => MakeRequest(memberId, username, amount, provider));
        }

        private static async void MakeRequest(int memberId, string username, decimal amount, string provider)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.GetAsync(new Uri("http://www.woopra.com/track/ce/?host=baymavi.com&response=json&timeout=300000&cv_id=" + memberId + "&cv_name=" + username + "&event=deposit&ce_pprovider=" + provider + "&ce_amount=" + amount));
                }
                catch (System.Exception ex)
                {

                }
                finally
                {
                    httpClient.Dispose();
                }
            }
        }

        private static async void MakeCustomRequest(string customAction, int memberId, string username, string data)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.GetAsync(new Uri("http://www.woopra.com/track/ce/?host=baymavi.com&response=json&timeout=300000&cv_id=" + memberId + "&cv_name=" + username + "&event=" + customAction + "&ce_affcode=" + data));
                }
                catch (System.Exception ex)
                {

                }
                finally
                {
                    httpClient.Dispose();
                }
            }
        }
    }
}