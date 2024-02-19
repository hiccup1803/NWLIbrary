using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class AstroPayRequest : Entity<int> 
    {
        public virtual int MemberId { get; set; }
        public virtual decimal CardAmount { get; set; }
        public virtual decimal WalletAmount { get; set; }
        
        public virtual DateTime CreateDate { get; set; }


        public virtual string CardNumber { get; set; }
        public virtual string CardCvv { get; set; }
        public virtual string CardCurrency { get; set; }
        public virtual int CardExpMonth { get; set; }
        public virtual int CardExpYear { get; set; }
        public virtual int Type { get; set; }
        public virtual int Status { get; set; }
        public virtual bool? WithBonus { get; set; }
        public virtual int? BonusId { get; set; }
    }
}
