using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Tag
{
    public class MemberTagFilterMap : ClassMap<MemberTagFilter>
    {

        public MemberTagFilterMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.FilterQuery);
            Map(x => x.FilterQueryJSON);
            Map(x => x.FilterQuerySQL);
            Map(x => x.PowerUserId);
            Map(x => x.StatusType);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.ExecutionType);

            References(x => x.PowerUser).Column("PowerUserId").Not.LazyLoad().Fetch.Join().ReadOnly();

            Table("MemberTagFilter");
        }
    }
}
