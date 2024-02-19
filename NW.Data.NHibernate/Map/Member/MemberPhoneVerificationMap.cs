using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using NW.Core.Entities;

namespace NW.Data.NHibernate.Map.Member
{
    class MemberPhoneVerificationMap : ClassMap<MemberPhoneVerification>
    {
        public MemberPhoneVerificationMap()
        {
            Id(l => l.Id);
            Map(l => l.MemberId);
            Map(l => l.CreateDate);
            Map(l => l.Verified);
            Map(l => l.VerifyDate);
            Map(l => l.Code);
            Map(l => l.Phone);
            Map(l => l.SentCount);
        }
    }
}
