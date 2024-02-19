using FluentNHibernate.Mapping;
using NW.Core.Entities.Marketing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Marketing
{
    public class BannerAnnotationMap : ClassMap<BannerAnnotation>
    {

        public BannerAnnotationMap()
        {
            Id(x => x.Id);
            Map(x => x.MarketingBannerSizeId);
            Map(x => x.UrlAlias);
            Map(x => x.UrlToRedirect);
            Map(x => x.CreateDate);
            References(x => x.MarketingBannerSize).Column("MarketingBannerSizeId").ReadOnly();

            Table("MarketingBannerAnnotation");
        }
    }
}
