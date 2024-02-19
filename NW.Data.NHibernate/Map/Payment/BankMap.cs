using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class BankMap: ClassMap<NW.Core.Entities.Payment.Bank>
    {
        public BankMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.StatusType);
            Map(x => x.CreateDate);
            Map(x => x.Logo);


            HasMany(x => x.BankTransferBankAccounts).KeyColumn("BankId").Inverse().Cascade.All();

            Table("Bank");
        }
    }
}
