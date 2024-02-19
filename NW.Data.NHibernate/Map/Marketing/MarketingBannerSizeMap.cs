using FluentNHibernate.Mapping;
using NW.Core.Entities.Marketing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Marketing
{
    public class MarketingBannerSizeMap : ClassMap<MarketingBannerSize>
    {
        public MarketingBannerSizeMap()
        {
            Id(x => x.Id);
            Map(x => x.MarketingBannerId);
            Map(x => x.Images);
            Map(x => x.IframeUrl);
            Map(x => x.Url);
            Map(x => x.Width);
            Map(x => x.Height);
            Map(x => x.CreateDate);
            Map(x => x.StatusType);

             
            References(x => x.MarketingBanner).Column("MarketingBannerId").ReadOnly();

            HasOne(x => x.BannerAnnotation).Cascade.All().PropertyRef("MarketingBannerSizeId");
            HasMany(x => x.MarketingBannerSizeItems).KeyColumn("MarketingBannerSizeId").Inverse().Cascade.All();


            Table("MarketingBannerSize");

        }

    }
}
