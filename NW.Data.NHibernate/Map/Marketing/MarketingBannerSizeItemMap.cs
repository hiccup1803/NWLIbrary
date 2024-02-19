using FluentNHibernate.Mapping;
using NW.Core.Entities.Marketing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Marketing
{

    public class MarketingBannerSizeItemMap : ClassMap<MarketingBannerSizeItem>
    {
        public MarketingBannerSizeItemMap()
        {
            Id(x => x.Id);
            Map(x => x.MarketingBannerSizeId);
            Map(x => x.ImagePath);
            Map(x => x.Url);
            Map(x => x.DisplayOrder);
            Map(x => x.CreateDate);
            Map(x => x.StatusType);


            References(x => x.MarketingBannerSize).Column("MarketingBannerSizeId").ReadOnly();



            Table("MarketingBannerSizeItem");

        }

    }
}
