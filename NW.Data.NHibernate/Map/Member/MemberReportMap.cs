using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class MemberReportMap : ClassMap<NW.Core.Entities.MemberReport>
    {
        public MemberReportMap()
        {
            Id(x => x.Id);
            Map(x => x.Username);
            Map(x => x.Email);
            Map(x => x.StatusType);
            Map(x => x.Password);
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.UniqueId);
            Map(x => x.CompanyId);
            Map(x => x.LevelId);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.Currency);
            Map(x => x.Host);
            Map(x => x.AffCode);
            Map(x => x.IsTestAccount, "IsTest");
            Map(x => x.VoltronMemberId);
            Map(x => x.EmailVerified);


            Table("MemberReportView");
            ReadOnly();
        }
    }
}
