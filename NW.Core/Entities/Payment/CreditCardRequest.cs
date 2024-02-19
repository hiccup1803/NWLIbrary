using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class CreditCardRequest : Entity<int> 
    {
        public virtual int PaymentProviderId { get; set; }
        public virtual int MemberId { get; set; }
        public virtual int CCId { get; set; }
        public virtual Int64 OriginalAmount { get; set; }
        public virtual Int64 Amount { get; set; }

        public virtual string ProviderRefId { get; set; }
        public virtual string ProviderRefEpro { get; set; }
        public virtual string Request { get; set; }
        public virtual bool IsComplete { get; set; }
      
        public virtual DateTime CreateDate { get; set; }
        public virtual bool? WithBonus { get; set; }
        public virtual int? BonusId { get; set; }
    }
}
