using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class MemberLoginCodeMap: ClassMap<MemberLoginCode>
    {
        public MemberLoginCodeMap()
        {
            Id(x => x.Id);
            Map(x => x.LoginCode);
            Map(x => x.StatusType);
            Map(x => x.MemberId);
            Map(x => x.CreateDate);
            Map(x => x.TryCount);
            Map(x => x.TryLoginCodeCount);

            Table("MemberLoginCode");
        }
    }
}
