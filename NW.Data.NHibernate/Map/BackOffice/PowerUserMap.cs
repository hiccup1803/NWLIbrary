using FluentNHibernate.Mapping;
using NW.Core.Entities.BackOffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.BackOffice
{
    public class PowerUserMap : ClassMap<PowerUser>
    {
        public PowerUserMap()
        {
            Id(x => x.Id);
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Email);
            Map(x => x.UserName);
            Map(x => x.Password);
            Map(x => x.BackOfficeRoleId);
            Map(x => x.CreateDate);
            Map(x => x.StatusType);
            Map(x => x.IPAddresses);
            Map(x => x.PhoneNumber);
            Map(x => x.LoginCode);
            Map(x => x.SecretCode);

            HasManyToMany(x => x.RestrictedCompanies)
                .Cascade.All()
                .Table("PowerUserRestrictedCompany")
                .ParentKeyColumn("PowerUserId")
                .ChildKeyColumn("CompanyId");

            HasManyToMany(x => x.Providers)
                .Cascade.All()
                .Table("PowerUserProvider")
                .ParentKeyColumn("PowerUserId")
                .ChildKeyColumn("ProviderId");

            HasManyToMany(x => x.Members)
                .Cascade.All()
                .Table("PowerUserMemberLimit")
                .ParentKeyColumn("PowerUserId")
                .ChildKeyColumn("MemberId");

            References(pu => pu.BackOfficeRole).Column("BackOfficeRoleId").ReadOnly();

            Table("PowerUser");
        }
    }
}
