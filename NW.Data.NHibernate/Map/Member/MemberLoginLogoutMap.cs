using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class MemberLoginLogoutMap: ClassMap<MemberLoginLogout>
    {
        public MemberLoginLogoutMap()
        {
            Id(l => l.Id);
            Map(l => l.MemberId);
            Map(l => l.LoginDate);
            Map(l => l.LogoutDate);
        }
    }
}