using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Payment.Wrappers.AstroPay
{

    /// <summary>
    /// 
    /// </summary>
    public class AstroRequest
    {
        
        ///<summary>
        /// 
        /// </summary>
        public string Login { get; set; }

        ///<summary>
        /// 
        /// </summary>
        public string TranKey { get; set; }

        ///<summary>
        /// 
        /// </summary>
        public string Version { get; set; }


        ///<summary>
        /// 
        /// </summary>
        public string Type { get; set; }


        ///<summary>
        /// 
        /// </summary>
        public string TestRequest { get; set; }

        /// <summary>
        /// Card number. 16 digit integer acording to the API document but we don't check that.
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Security code of the card. 3-4 digit integer.
        /// </summary>
        public int CardCvv { get; set; }

        public string CardExpDate { get; set; } 
        ///<summary>
        /// 
        /// </summary>

        /// <summary>
        /// Amount of the transaction. MUST BE IN USD
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Before we convert it to 
        /// </summary>
        public decimal ActualAmount { get; set; }

        /// <summary>
        /// Unique, anonymized identifier of users in themerchantsystem
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Unique identifier of Merchant transaction
        /// </summary>
        public string InvoiceNum { get; set; }

        /// <summary>
        /// The time window after a transaction is taken as duplicated
        /// </summary>
        public int DuplicateWindow { get; set; }

        /// <summary>
        /// Response delimiting char between fields when the x_response_format is “string”
        /// </summary>
        public string DelimChar { get; set; }

        /// <summary>
        /// The response format. Options: “string”, “json”, “xml”
        /// </summary>
        public string ResponseFormat { get; set; }


        /// <summary>
        /// The request to receive a delimited transaction response
        /// </summary>
        public bool DelimData { get; set; }

        /// <summary>
        ///Type of pay method
        /// </summary>
        public string  Method { get; set; }

        /// <summary>
        ///(For VOID, CREDIT and REFUND only) TransactionIDparam returned by AstroPay on AUTH_CAPTURE or CAPTURE_ONLY previous tranasction
        /// </summary>
        public string TransId { get; set; }

        /// <summary>
        /// (For CAPTURE_ONLY only) TransactionIDparam sent by AstroPay in previous AUTH_ONLY call
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        
        /// </summary>
        public string CustId { get; set; }

        /// <summary>
        /// Customer Id
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Customer firstname
        /// </summary>
        public string Firstname { get; set; }

        /// <summary>
        /// Customer lastanme
        /// </summary>
        public string Lastname { get; set; }
         

        /// <summary>
        /// Client IP Address.
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// Currency. ISO4217 http://www.xe.com/iso4217.php (USD, GBP, EUR) in our system.
        /// </summary>
        public string Currency { get; set; }
        //public string CardOwner { get; set; }

        /// <summary>
        /// This is your return url when the 3ds completes.
        /// </summary>
        public string ReturnUrl { get; set; }
         
    }
}
