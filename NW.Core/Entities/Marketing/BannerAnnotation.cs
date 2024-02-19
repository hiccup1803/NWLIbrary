using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Marketing
{

    public class BannerAnnotation : Entity<int>
    {
        public virtual int MarketingBannerSizeId { get; set; }
        public virtual string UrlAlias { get; set; }
        public virtual string UrlToRedirect { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual MarketingBannerSize MarketingBannerSize { get; set; }
    }
}
