using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class BankTransferRequest : Entity<int>
    {
        public virtual int PaymentStatusType { get; set; }
        public virtual int BankTransferBankAccountId { get; set; }
        public virtual int MemberId { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual int? UpdateBy { get; set; }
        public virtual long Amount { get; set; }
        public virtual long UpdateAmount { get; set; }
        public virtual DateTime TransferDate { get; set; }
        public virtual int TransferWayType { get; set; }
        public virtual string IdentityNumber { get; set; }
        public virtual string Bank { get; set; }
        public virtual string IBAN { get; set; }
        public virtual string BranchCode { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string SenderFullName { get; set; }
        public virtual bool WithBonus { get; set; }
        public virtual int? BonusId { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual string Note { get; set; }
    }
}

