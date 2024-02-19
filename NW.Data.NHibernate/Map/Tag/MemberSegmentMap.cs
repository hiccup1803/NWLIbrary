using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Tag
{
    public class MemberSegmentMap : ClassMap<MemberSegment>
    {

        public MemberSegmentMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.FilterQueryJSON);
            Map(x => x.FilterQuerySQL);
            Map(x => x.PowerUserId);
            Map(x => x.StatusType);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.CronExpression);

            //References(x => x.PowerUser).Column("PowerUserId").Not.LazyLoad().Fetch.Join().ReadOnly();


            HasManyToMany(x => x.Banners)
                .Cascade.All()
                .Table("MemberSegmentCMS_Banner")
                .ParentKeyColumn("MemberSegmentId")
                .ChildKeyColumn("CMS_BannerId");

            Table("MemberSegment");
        }
    }
}
