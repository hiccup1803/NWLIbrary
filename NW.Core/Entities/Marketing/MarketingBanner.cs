using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Marketing
{
    public class MarketingBanner : Entity<int>
    {
        public virtual string Title { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int StatusType { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual IList<MarketingBannerSize> MarketingBannerSizes { get; set;}
    }
}
