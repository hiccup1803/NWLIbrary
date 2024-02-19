using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class MemberTrackingMap: ClassMap<MemberTracking>
    {
        public MemberTrackingMap()
        {
            Id(l => l.Id);
            Map(l => l.MemberId);
            Map(l => l.Domain);
            Map(l => l.AbsoluteUri);
            Map(l => l.Ip);
            Map(l => l.CreateDate);
        }
    }
}