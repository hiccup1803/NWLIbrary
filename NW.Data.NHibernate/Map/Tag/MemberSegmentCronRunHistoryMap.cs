using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Tag
{
    public class MemberSegmentCronRunHistoryMap : ClassMap<MemberSegmentCronRunHistory>
    {

        public MemberSegmentCronRunHistoryMap()
        {
            Id(x => x.Id);
            Map(x => x.MemberSegmentId);
            Map(x => x.CreateDate);
            Map(x => x.QueryResultCount);
            Map(x => x.DowngradeMemberCount);
            Map(x => x.UpgradeMemberCount);


            Table("MemberSegmentCronRunHistory");
        }
    }
}
