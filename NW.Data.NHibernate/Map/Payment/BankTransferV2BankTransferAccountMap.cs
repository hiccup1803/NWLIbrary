using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    class BankTransferV2BankTransferAccountMap : ClassMap<NW.Core.Entities.Payment.BankTransferV2BankTransferAccount>
    {
        public BankTransferV2BankTransferAccountMap()
        {
            Id(x => x.Id);
            Map(x => x.StatusType);
            Map(x => x.CreateDate);
            Map(x => x.NameSurname);
            Map(x => x.AccountNumber);
            Map(x => x.IBAN);
            Map(x => x.BranchCode);
            Map(x => x.ReferenceNote);
            Map(x => x.BankId);
            Map(x => x.CompanyId);
            Map(x => x.ProviderId);


            //HasManyToMany(x => x.Levels)
            //    .Cascade.All()
            //    .Table("PaparaTransferPaparaAccountLevel")
            //    .ParentKeyColumn("PaparaTransferPaparaAccountId")
            //    .ChildKeyColumn("LevelId");

            References(m => m.Company).Column("CompanyId").ReadOnly();
            References(m => m.Provider).Column("ProviderId").ReadOnly();
            References(m => m.Bank).Column("BankId").ReadOnly();

            Table("BankTransferV2BankTransferAccount");
        }
    }
}