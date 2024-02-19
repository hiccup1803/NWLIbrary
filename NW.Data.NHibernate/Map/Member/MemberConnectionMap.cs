using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using NW.Core.Entities;

namespace NW.Data.NHibernate.Map.Member
{
    public class MemberConnectionMap : ClassMap<MemberConnection>
    {
        public MemberConnectionMap()
        {
            Id(l => l.Id);
            Map(l => l.Device);
            Map(l => l.MemberId);
            Map(l => l.Username);
            Map(l => l.CurrentUrl);
            Map(l => l.IsOnline);
            Map(l => l.IP);
            Map(l => l.UpdateDate);
            Map(l => l.CreateDate);
            Map(l => l.ConnectionId);
        }
    }
}
