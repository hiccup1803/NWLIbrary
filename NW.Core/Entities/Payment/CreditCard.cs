using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class CreditCard : Entity<int> 
    {
        public virtual int MemberId { get; set; }
        public virtual int StatusType { get; set; }
        public virtual string NameOnCard { get; set; }
        public virtual string CardNumber { get; set; }
        public virtual string CVV { get; set; }
        public virtual int ExpiryMonth { get; set; }
        public virtual int ExpiryYear { get; set; }

        public virtual string CardTypeString
        {
            get { return CardNumber.StartsWith("5") ? "master" :"visa"; }
        }

        public virtual string MaskedCardNumber
        {
            get { return "************" + CardNumber.Substring(CardNumber.Length - 4, 4); }
        }

        public virtual DateTime CreateDate { get; set; }
    }
}
