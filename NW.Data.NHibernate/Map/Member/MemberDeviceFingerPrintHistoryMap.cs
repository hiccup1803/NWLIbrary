using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class MemberDeviceFingerPrintHistoryMap : ClassMap<MemberDeviceFingerPrintHistory>
    {
        public MemberDeviceFingerPrintHistoryMap()
        {
            Id(l => l.Id);
            Map(l => l.MemberId);
            Map(l => l.Hash);
            Map(l => l.StatusType);
            Map(l => l.CreateDate);
            Map(l => l.UpdateDate);
            Map(l => l.Geolocation);
            Map(l => l.IP);
            Map(l => l.IsLoggedIn);

            References(m => m.Member).Column("MemberId").ReadOnly();
        }
    }
}
