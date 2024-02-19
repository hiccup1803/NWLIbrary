using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Marketing
{
    public class MarketingBannerSizeItem : Entity<int>
    {
        public virtual int MarketingBannerSizeId { get; set; }
        public virtual string ImagePath { get; set; }
        public virtual string Url { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int StatusType { get; set; }
        public virtual MarketingBannerSize MarketingBannerSize { get; set; }
    }
}
