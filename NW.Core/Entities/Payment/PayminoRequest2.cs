using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class PayminoRequest2 : Entity<int>
    {
        public virtual int StatusType { get; set; }
        public virtual int RequestType { get; set; }
        public virtual int PaymentMethod { get; set; }
        public virtual int MemberId { get; set; }
        public virtual string Firstname { get; set; }
        public virtual string Lastname { get; set; }
        public virtual long Amount { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int PaymentProviderTransactionId { get; set; }
        public virtual DateTime? UpdateDate { get; set; }
        public virtual string ResponseJson { get; set; }
        public virtual bool? WithBonus { get; set; }
        public virtual int? BonusId { get; set; }
        public virtual long RecognisedAmount { get; set; }
    }
}
