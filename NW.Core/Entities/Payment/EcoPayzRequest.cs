using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW.Core.Enum;

namespace NW.Core.Entities.Payment
{
    public class EcoPayzRequest : Entity<int> 
    {
        public virtual int MemberId { get; set; }
        public virtual int TxBatchNumber { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual string Currency { get; set; }
        public virtual DateTime CreateDate { get; set; }



        public virtual bool ProcessedByEco { get; set; }
        public virtual long EcoTxId { get; set; }
        public virtual string EcoXml { get; set; }
        public virtual DateTime EcoProcessedDate { get; set; }

        public virtual int EcoClientAccountNumber { get; set; }
        public virtual int WithdrawStatusType { get; set; }
        public virtual long PaymentTransactionId { get; set; }

        // 0=deposit, 1=withdraw
        public virtual int RequestType { get; set; }
        public virtual bool? WithBonus { get; set; }
        public virtual int? BonusId { get; set; }

    }
}
