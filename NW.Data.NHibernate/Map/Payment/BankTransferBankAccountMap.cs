using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class BankTransferBankAccountMap : ClassMap<NW.Core.Entities.Payment.BankTransferBankAccount>
    {
        public BankTransferBankAccountMap()
        {
            Id(x => x.Id);
            Map(x => x.BankId);
            Map(x => x.StatusType);
            Map(x => x.CreateDate);
            Map(x => x.NameSurname);
            Map(x => x.NameSurnameMasked);
            Map(x => x.Branch);
            Map(x => x.BranchCode);
            Map(x => x.AccountNumber);
            Map(x => x.IBAN);
            Map(x => x.BlaclistedUsernameList).Length(4001);
            Map(x => x.CompanyId);


            HasManyToMany(x => x.Levels)
                .Cascade.All()
                .Table("BankTransferBankAccountLevel")
                .ParentKeyColumn("BankTransferBankAccountId")
                .ChildKeyColumn("LevelId");

            References(m => m.Bank).Column("BankId").ReadOnly();

            Table("BankTransferBankAccount");
        }
    }
}
