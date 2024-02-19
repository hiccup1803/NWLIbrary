using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using NW.Core.Entities;

namespace NW.Data.NHibernate.Map.Member
{
    public class MemberNotificationMap : ClassMap<MemberNotification>
    {
        public MemberNotificationMap()
        {
            Id(l => l.Id);
            Map(l => l.MemberId);
            Map(l => l.CreateDate);
            Map(l => l.DesktopPushEnabled);
            Map(l => l.FacebookPushEnabled);
            Map(l => l.MobilePushEnabled);
            Map(l => l.IsGCM);
            Map(l => l.Endpoint);
        }
    }
}
