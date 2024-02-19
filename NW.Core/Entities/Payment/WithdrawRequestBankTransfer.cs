using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class WithdrawRequestBankTransfer : Entity<int>
    {
        public virtual int WithdrawStatusType { get; set; }
        public virtual long? PaymentTransactionId { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual int MemberId { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual string Bank { get; set; }
        public virtual string IBAN { get; set; }
        public virtual string BranchCode { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string Currency { get; set; }
    }
}
