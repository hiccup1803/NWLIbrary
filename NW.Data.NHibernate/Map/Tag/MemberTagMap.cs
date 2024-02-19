using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Tag
{
    public class MemberTagMap : ClassMap<MemberTag>
    {

        public MemberTagMap()
        {
            Id(x => x.Id);
            Map(x => x.TagId);
            Map(x => x.MemberId);
            Map(x => x.Active);
            Map(x => x.CreateDate);

            References(x => x.Tag).Column("TagId").Cascade.None().ReadOnly();
            References(x => x.Member).Column("MemberId").Cascade.None().ReadOnly();

            Table("MemberTag");
        }
    }
}
