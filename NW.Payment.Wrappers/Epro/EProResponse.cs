using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace NW.Payment.Wrappers.EProPayment
{
    public class EProResponse
    {
        /// <summary>
        /// The API request status. It only indicates if your request has been successfully transmitted.
        /// pending, failed, captured
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Your own transaction identifier.
        /// </summary>
        public string Tid { get; set; }

        /// <summary>
        /// E-PRO unique reference identifier.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Amount of the transaction.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Your unique user identifier.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// This message give you more information about the current action.
        /// EX: Waiting 3Dsecure validation
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// This error is provided by the bank.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// The 3DSecure URL where the client must be redirected.
        /// </summary>
        public string ThreeDSecureUrl { get; set; }

        /// <summary>
        /// The transaction date.
        /// </summary>
        public string Date { get; set; }

        public EProResponse(NameValueCollection form)
        {
            Status = form["Status"];
            Tid = form["Tid"];
            Reference = form["Reference"];
            try
            {
                Amount = Convert.ToInt32(form["Amount"]);
                // when returns from 3ds amount is like 12.00 hence error when convert to integer.
            }
            catch {}
            //UserId = form["UserId"];
            //Message = form["Message"];
            //Error = form["Error"];
            //ThreeDSecureUrl = form["3DSecureUrl"];
            //Date = form["Date"];
        }
    }


}
