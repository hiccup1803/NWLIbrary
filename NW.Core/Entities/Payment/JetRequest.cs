using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class JetRequest : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual int StatusType { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual string Currency { get; set; }
        public virtual long? JetPaymentId { get; set; }
        public virtual string JetToken { get; set; }

        public virtual string JetResult { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string JetClientAccountNumber { get; set; }
        public virtual long? PaymentTransactionId { get; set; }
        public virtual bool? WithBonus { get; set; }
        public virtual int? BonusId { get; set; }
        public virtual string CallbackData { get; set; }
        public virtual DateTime? UpdateDate { get; set; }
        public virtual long RecognisedAmount { get; set; }

    }
}
