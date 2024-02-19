using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class NewPaymentProvider : Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual int StatusType { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string Currency { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual int? VoltronProviderId { get; set; }
        public virtual int? ParentProviderId { get; set; }
        public virtual string ThumbnailName { get; set; }
        public virtual string SystemName { get; set; }
        public virtual decimal? MinAmount { get; set; }
        public virtual decimal? MaxAmount { get; set; }
        public virtual int? Weight { get; set; }
        public virtual string HelplinkId { get; set; }
        public virtual string ClassName { get; set; }
        public virtual string ClosedCron { get; set; }
        public virtual string ResourceName { get; set; }
    }
}
