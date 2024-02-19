using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace NW.Payment.Wrappers.AstroPay
{
    public class AstroResponse
    {
        /// <summary>
        /// Transaction response code 1-Approved 2-Declined 3-Error
        /// </summary>
        public int ResponseCode { get; set; }

        /// <summary>
        /// Subcode response (see response codes table)
        /// </summary>
        public int ResponseSubcode { get; set; }

        /// <summary>
        /// (See response codestable)
        /// </summary>
        public int ResponseReasonCode { get; set; }

        /// <summary>
        /// Description of the transaction result
        /// “Transaction OK!”
        /// </summary>
        public string ResponseReasonText { get; set; }

        /// <summary>
        /// Authrizationcode
        /// </summary>
        public string ApprovalCode { get; set; }

        /// <summary>
        /// AVS of thetransaction
        /// </summary>
        public string AVS { get; set; }

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

        /// <summary>
        /// TAstropay user who's using thecard (Numeric) (Only for version 2.0 or greater
        /// </summary>
        public string InvoiceNum { get; set; }

        // <summary>
        /// Description of the transaction provided by the merchant
        /// </summary>
        public string Description { get; set; }



        // <summary>
        /// The amount of the transaction. Important: For safety reasons, it’s highly recommended compare the amount sent with the response amount
        /// </summary>
        public decimal Amount { get; set; }


        /// <summary>
        ///Transaction type (AUTH_CAPTURE, etc.)
        /// string. Options: (“AUTH_ONLY”, “CAPTURE_ONLY”, “AUTH_CAPTURE”, “CREDIT”, “REFUND”, “VOID”)
        /// </summary>
        public string Type { get; set; }


        /// <summary> 
        /// </summary>
        public string CustId { get; set; }

        /// <summary>
        /// Generated MD5 hash value that may be used to authenticate thetransaction response (see 2.2 String control MD5 hash for return post)
        /// </summary>
        public string Md5Hash { get; set; }

        /// <summary>
        /// The card code verification (CCV) response code M = Match N = No Match P = Not Processed S = Should have been present U = Issuer unable to process request
        /// Options: (“M”, “N”, “P”, ”S”, ”U”)
        /// </summary>
        public string CcResponse { get; set; }


        //public AstroResponse(NameValueCollection form)
        //{
        //    Status = form["Status"];
        //    Tid = form["Tid"];
        //    Reference = form["Reference"];
        //    try
        //    {
        //        Amount = Convert.ToInt32(form["Amount"]);
        //        // when returns from 3ds amount is like 12.00 hence error when convert to integer.
        //    }
        //    catch {}
        //    //UserId = form["UserId"];
        //    //Message = form["Message"];
        //    //Error = form["Error"];
        //    //ThreeDSecureUrl = form["3DSecureUrl"];
        //    //Date = form["Date"];
        //}
    }


}
