using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class DepositBonusHistory : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual int VoltronTransactionTypeId { get; set; }
        public virtual int PaymentProviderId { get; set; }

        public virtual long Amount { get; set; }
        public virtual DateTime CreateDate { get; set; }
    }
}
