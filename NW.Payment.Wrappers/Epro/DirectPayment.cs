using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Payment.Wrappers.EProPayment
{

    /// <summary>
    /// 
    /// </summary>
    public class DirectPayment
    {
        /// <summary>
        /// Amount of the transaction. MUST BE IN EUR
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Before we convert it to EUR
        /// </summary>
        public int ActualAmount { get; set; }

        /// <summary>
        /// Client identifier. Member id would fit here.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Transaction identifier. Send something unique.
        /// </summary>
        public string Tid { get; set; }

        /// <summary>
        /// ??
        /// </summary>
        public string BirthDate { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }


        /// <summary>
        /// Customer email address
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
        /// Card number. 16 digit integer acording to the API document but we don't check that.
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Month of the card. 2 digit integer. eg: 09
        /// </summary>
        public string CardMonth { get; set; }

        /// <summary>
        /// Year of the card. 4 digit integer. 2014
        /// </summary>
        public int CardYear { get; set; }

        /// <summary>
        /// Security code of the card. 3-4 digit integer.
        /// </summary>
        public string CardCvv { get; set; }

        /// <summary>
        /// Mastercard,Visa only. Not mandatory
        /// </summary>
        public string CardType { get; set; }
        //public string CardOwner { get; set; }

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

        public string ZipCode { get; set; }

        /// <summary>
        /// Posible values are yes|no
        /// </summary>
        public string ThreeDs { get; set; }
    }
}
