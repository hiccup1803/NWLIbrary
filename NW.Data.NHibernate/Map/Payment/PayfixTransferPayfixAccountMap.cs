using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    class PayfixTransferPayfixAccountMap : ClassMap<NW.Core.Entities.Payment.PayfixTransferPayfixAccount>
    {
        public PayfixTransferPayfixAccountMap()
        {
            Id(x => x.Id);
            Map(x => x.StatusType);
            Map(x => x.CreateDate);
            Map(x => x.NameSurname);
            Map(x => x.NameSurnameMasked);
            Map(x => x.AccountNumber);
            Map(x => x.BlaclistedUsernameList).Length(4001);
            Map(x => x.CompanyId);
            Map(x => x.ProviderId);


            HasManyToMany(x => x.Levels)
                .Cascade.All()
                .Table("PayfixTransferPayfixAccountLevel")
                .ParentKeyColumn("PayfixTransferPayfixAccountId")
                .ChildKeyColumn("LevelId");

            References(m => m.Company).Column("CompanyId").ReadOnly();
            References(m => m.Provider).Column("ProviderId").ReadOnly();

            Table("PayfixTransferPayfixAccount");
        }
    }
}