using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map
{
    public class BannerMap : ClassMap<Banner>
    {

        public BannerMap()
        {
            Id(x => x.Id);
            Map(x => x.Title);
            Map(x => x.BannerPlace);
            Map(x => x.BannerType);
            Map(x => x.LanguageId);
            Map(x => x.BannerUrl);
            Map(x => x.MobileUrl);
            Map(x => x.BannerImagePath);
            Map(x => x.MobileBannerImagePath);
            Map(x => x.MemberTagFilterId);
            Map(x => x.CompanyId);
            Map(x => x.Active);
            Map(x => x.IsVip);
            Map(x => x.DisplayOrder);
            Map(x => x.BannerDay);
            Map(x => x.StartDate);
            Map(x => x.EndDate);
            Map(x => x.StartTime).CustomType("TimeAsTimeSpan");
            Map(x => x.EndTime).CustomType("TimeAsTimeSpan");
            Map(x => x.BannerUsernameFilterType);
            Map(x => x.UsernameList).Length(4001);
            Map(x => x.CreateDate);
            Map(x => x.StatusType);

            References(x => x.Language).Column("LanguageId").ReadOnly();
            References(x => x.Company).Column("CompanyId").ReadOnly();
            References(x => x.MemberTagFilter).Column("MemberTagFilterId").Not.LazyLoad().Fetch.Join().ReadOnly();



            HasManyToMany(x => x.MemberSegments)
                .Cascade.All()
                .Table("MemberSegmentCMS_Banner")
                .ParentKeyColumn("CMS_BannerId")
                .ChildKeyColumn("MemberSegmentId");

            Table("CMS_Banner");
        }
    }
}
