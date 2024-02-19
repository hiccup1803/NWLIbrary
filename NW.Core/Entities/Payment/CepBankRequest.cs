using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class CepBankRequest : Entity<int>
    {
        public virtual int PaymentProviderId { get; set; }
        public virtual int CepBankId { get; set; }
        public virtual int MemberId { get; set; }
        public virtual long? PaymentTransactionId { get; set; }
        public virtual string SenderId { get; set; }
        public virtual string ReceipientId { get; set; }
        public virtual string SenderPhone { get; set; }
        public virtual string ReceipientPhone { get; set; }
        public virtual string ReceipientBirthday { get; set; }
        public virtual string Password { get; set; }
        public virtual long Amount { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int PaymentStatusType { get; set; }
        public virtual bool WithBonus { get; set; }
        public virtual int? BonusId { get; set; }
        public virtual string ProviderRefId { get; set; }

        public virtual CepBank CepBank { get; set; }
        public virtual Member Member { get; set; }
    }
}
