using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Tag
{
    public class MemberSegmentMemberMap : ClassMap<MemberSegmentMember>
    {

        public MemberSegmentMemberMap()
        {
            Id(x => x.Id);
            Map(x => x.MemberSegmentId);
            Map(x => x.MemberId);
            Map(x => x.StatusType);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);


            References(m => m.MemberSegment).Column("MemberSegmentId").ReadOnly();

            Table("MemberSegmentMember");
        }
    }
}
