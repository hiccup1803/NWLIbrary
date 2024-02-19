using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Marketing
{
    public class MarketingBannerSize : Entity<int>
    {
        public virtual int MarketingBannerId { get; set; }
        public virtual string Images { get; set; }
        public virtual string IframeUrl { get; set; }
        public virtual string Url { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int StatusType { get; set; }
        public virtual MarketingBanner MarketingBanner { get; set; }
        public virtual BannerAnnotation BannerAnnotation { get; set; }
        public virtual IList<MarketingBannerSizeItem> MarketingBannerSizeItems { get; set; }
    }
}
