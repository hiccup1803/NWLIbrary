using FluentNHibernate.Mapping;
using NW.Core.Entities.Marketing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Marketing
{
    public class MarketingBannerMap : ClassMap<MarketingBanner>
    {
        public MarketingBannerMap()
        {
            Id(x => x.Id);
            Map(x => x.Title);
            Map(x => x.CreateDate);
            Map(x => x.CompanyId);
            Map(x => x.StatusType);


            HasMany(x => x.MarketingBannerSizes).KeyColumn("MarketingBannerId").Inverse().Cascade.All();


            Table("MarketingBanner");

        }

    }
}
