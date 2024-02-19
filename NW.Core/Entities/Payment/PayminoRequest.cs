using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class PayminoRequest : Entity<int>
    {
        public virtual int StatusType { get; set; }
        public virtual int BankId { get; set; }
        public virtual int MemberId { get; set; }
        public virtual string Firstname { get; set; }
        public virtual string Lastname { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int PaymentProviderTransactionId { get; set; }
        public virtual DateTime? UpdateDate { get; set; }
    }
}
