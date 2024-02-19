using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class OtoPayTransaction : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual decimal TLAmount { get; set; }
        public virtual string CardNumber { get; set; }
        public virtual string CVV { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime CreateDate { get; set; }

        public virtual int StatusType { get; set; }
        public virtual Int64 WalletTransactionRefId { get; set; }
        public virtual string Response { get; set; }
        public virtual DateTime? ResponseDate { get; set; }
        public virtual string OtoPayId { get; set; }
        public virtual string OtoPayReference { get; set; }

    }
}
