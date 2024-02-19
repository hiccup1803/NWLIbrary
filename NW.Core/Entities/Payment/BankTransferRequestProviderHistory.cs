using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class BankTransferRequestProviderHistory : Entity<int>
    {
        public virtual int ProviderId { get; set; }
        public virtual int BankTransferRequestId { get; set; }
        public virtual int ChangedBy { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual NewBankTransferRequest NewBankTransferRequest { get; set; }
    }
}
