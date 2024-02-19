﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class PEPTransferRequest : Entity<int>
    {
        public virtual int PaymentStatusType { get; set; }
        public virtual int PEPTransferPEPAccountId { get; set; }
        public virtual int MemberId { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual int? UpdateBy { get; set; }
        public virtual long Amount { get; set; }
        public virtual long UpdateAmount { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string SenderFullName { get; set; }
        public virtual bool WithBonus { get; set; }
        public virtual int? BonusId { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual int? ProviderId { get; set; }
        public virtual string Note { get; set; }
        public virtual Member Member { get; set; }
        public virtual PEPTransferPEPAccount PEPTransferPEPAccount { get; set; }
    }
}

