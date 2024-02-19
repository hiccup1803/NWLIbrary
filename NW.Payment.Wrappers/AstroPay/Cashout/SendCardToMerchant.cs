using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NW.Payment.Wrappers.AstroPay.Cashout
{
    public class SendCardToMerchant
    {
        ///<summary>
        /// 
        /// </summary>
        public string Login { get; set; }

        ///<summary>
        /// 
        /// </summary>
        public string TranKey { get; set; }
       
        /// <summary>
        /// Amount of the transaction. MUST BE IN USD
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Document { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Control {
            get { return HashCode("UdW8647WPzZIkOjttTI11qbHJG6NCE6A" + Amount + Currency + FullName); } 
        }


        public static string HashCode(string str)
        {
            var encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes(str);
            var cryptoTransformSha1 =new SHA1CryptoServiceProvider();
            string hash = BitConverter.ToString(
                cryptoTransformSha1.ComputeHash(buffer)).Replace("-", "");

            return hash;
        }

        
    }
}
