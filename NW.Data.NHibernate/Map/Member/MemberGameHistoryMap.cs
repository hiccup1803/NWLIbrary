using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class MemberGameHistoryMap : ClassMap<MemberGameHistory>
    {
        public MemberGameHistoryMap()
        {
            Id(x => x.Id);
            Map(x => x.GameId).Column("CasinoGameId");
            Map(x => x.MemberId);
            Map(x => x.CreateDate);
            Map(x => x.PFF);
            Map(x => x.SessionId);

            Table("MemberCasinoGameHistory");
        }
    }
}
