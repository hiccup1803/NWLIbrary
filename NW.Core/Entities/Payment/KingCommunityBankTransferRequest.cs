﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class KingCommunityBankTransferRequest : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual int StatusType { get; set; }
        public virtual long Amount { get; set; }
        public virtual string Currency { get; set; }
        public virtual string ProviderRefId { get; set; }
        public virtual string Bank { get; set; }
        public virtual string Data { get; set; }
        public virtual string ResultData { get; set; }
        public virtual string CallbackData { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime? UpdateDate { get; set; }
        public virtual bool? WithBonus { get; set; }
        public virtual int? BonusId { get; set; }
        public virtual long RecognisedAmount { get; set; }
    }
}
