using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class PaybinRequest : Entity<int>
    {
        public virtual int StatusType { get; set; }
        public virtual Int64 Amount { get; set; }
        public virtual Int64 ActualAmount { get; set; }
        public virtual int MemberId { get; set; }

        public virtual string Currency { get; set; }
        public virtual decimal OriginalAmount { get; set; }
        public virtual string Symbol { get; set; }
        public virtual string Data { get; set; }
        public virtual string UniqueId { get; set; }
        public virtual int OrderId { get; set; }
        public virtual string CallbackData { get; set; }
        public virtual DateTime CreateDate { get; set; }

        public virtual DateTime? UpdateDate { get; set; }
        public virtual bool? WithBonus { get; set; }
        public virtual int? BonusId { get; set; }
    }
}
