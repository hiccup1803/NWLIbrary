using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Payment.Wrappers.AstroPay
{
    public class AstroCard
    {
        ///<summary>
        /// 
        /// </summary>
        public string Code { get; set; }

        ///<summary>
        /// 
        /// </summary>
        public string Message { get; set; }
      
        /// <summary>
        /// 
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Before we convert it to 
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Unique identifier of Merchant transaction
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// The time window after a transaction is taken as duplicated
        /// </summary>
        public string CardExpiry { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CardCvv { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ControlSignature { get; set; }

     
    }
}
