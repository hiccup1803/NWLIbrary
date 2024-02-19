using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace NW.Payment.Wrappers.PayKasa
{
    public class PayKasaResponse
    {
        /// <summary>
        /// Transaction response code 1-Approved 2-Declined 3-Error
        /// </summary>
        public int ResponseCode { get; set; }

        /// <summary>
        /// This error is provided by the bank.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Transaction ID provided by AstroPay
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// TAstropay user who's using thecard (Numeric) (Only for version 2.0 or greater
        /// </summary>
        public int RUniqueId { get; set; }

        /// <summary>
        /// Invoice number provided by merchant
        /// </summary>
        public string UniqueId { get; set; }

        // <summary>
        /// The amount of the transaction. Important: For safety reasons, it’s highly recommended compare the amount sent with the response amount
        /// </summary>
        public decimal Amount { get; set; }


        /// <summary> 
        /// </summary>
        public string CustId { get; set; }

       
    }


}
