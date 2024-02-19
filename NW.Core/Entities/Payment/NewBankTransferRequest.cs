using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class NewBankTransferRequest : Entity<int>
    {
        public virtual int ProviderId { get; set; }
        public virtual int PaymentStatusType { get; set; }
        public virtual int BankTransferBankAccountId { get; set; }
        public virtual int ReceiverBankId { get; set; }
        public virtual Bank SenderBank { get; set; }
        public virtual Bank ReceiverBank { get; set; }
        public virtual string ReceiverNameSurname { get; set; }
        public virtual string ReceiverNameSurnameMasked { get; set; }
        public virtual string ReceiverBranch { get; set; }
        public virtual string ReceiverBranchCode { get; set; }
        public virtual string ReceiverAccountNumber { get; set; }
        public virtual string ReceiverIBAN { get; set; }
        public virtual int MemberId { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual int? UpdateBy { get; set; }
        public virtual long Amount { get; set; }
        public virtual long UpdateAmount { get; set; }
        //public virtual DateTime TransferDate { get; set; }
        public virtual int TransferWayType { get; set; }
        public virtual string IdentityNumber { get; set; }
        public virtual int SenderBankId { get; set; }
        public virtual string IBAN { get; set; }
        public virtual string BranchCode { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string SenderFullName { get; set; }
        public virtual bool WithBonus { get; set; }
        public virtual int? BonusId { get; set; }
        public virtual bool IsEFT { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual string Note { get; set; }
        public virtual IList<BankTransferRequestProviderHistory> ProviderHistory { get; set; }
    }
}
