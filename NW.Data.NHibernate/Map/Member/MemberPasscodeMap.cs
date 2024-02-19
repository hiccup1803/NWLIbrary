using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class MemberPasscodeMap: ClassMap<MemberPasscode>
    {
        public MemberPasscodeMap()
        {
            Id(l => l.Id);
            Map(l => l.StatusType);
            Map(l => l.MemberId);
            Map(l => l.Guid);
            Map(l => l.Device);
            Map(l => l.Passcode);
            Map(l => l.CreateDate);
        }
    }
}
