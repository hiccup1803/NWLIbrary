using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class BankTransferV2Request : Entity<int>
    {
        public virtual int PaymentStatusType { get; set; }
        public virtual int MemberId { get; set; }
        public virtual int PaymentProviderId { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual int? UpdateBy { get; set; }
        public virtual long Amount { get; set; }
        public virtual long UpdateAmount { get; set; }
        public virtual DateTime? TransferDate { get; set; }
        public virtual string SenderIdentityNumber { get; set; }
        public virtual string SenderFirstname { get; set; }
        public virtual string SenderLastname { get; set; }
        public virtual int RequestBankId { get; set; }
        public virtual bool FastEnabled { get; set; }
        public virtual int? ReceiverBankAccountId { get; set; }
        public virtual string ReceiverIBAN { get; set; }
        public virtual int ReceiverBankId { get; set; }
        public virtual string ReceiverBranchCode { get; set; }
        public virtual string ReceiverAccountNumber { get; set; }
        public virtual string ReceiverFullname { get; set; }
        public virtual string ReceiverReference { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual long RecognisedAmount { get; set; }
    }
}

