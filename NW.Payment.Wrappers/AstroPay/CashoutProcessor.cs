using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW.Payment.Wrappers.AstroPay.Cashout;

namespace NW.Payment.Wrappers.AstroPay
{
    public class CashoutProcessor
    {

        public dynamic Process(SendCardToMerchant data, string astroLogin, string astroTranKey, string astroURL)
        {
            try
            {
                //throw new Exception("Thrown exception intentionally by dev to process failover flow.");

                var httpHelper = new HttpHelper();
                var response = httpHelper.SendPostRequest(astroURL, Serialize(data, astroLogin, astroTranKey));
                return response;
            }
            catch (Exception ex)
            {
                return new { code = 0, reason_text = ex.Message };
            }

        }

        private dynamic Serialize(SendCardToMerchant data, string astroLogin, string astroTranKey)
        {

            if (data != null)
            {
                var sr = new StringBuilder();
                sr.Append("x_login=" + astroLogin);
                sr.Append("&x_tran_key=" + astroTranKey);
                sr.Append("&x_amount=" + data.Amount);
                sr.Append("&x_currency=" + data.Currency);
                sr.Append("&x_name=" + data.FullName);
                sr.Append("&x_document=" + data.Document);
                sr.Append("&x_country=" + data.Country);
                sr.Append("&x_control=" + data.Control);
                return sr.ToString();
            }
            return "";
        }
    }
}
