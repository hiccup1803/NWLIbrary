using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class BankAccountMap : ClassMap<NW.Core.Entities.Payment.BankAccount>
    {
        public BankAccountMap()
        {
            Id(x => x.Id);
            Map(x => x.MemberId);
            Map(x => x.IdentityNumber);
            Map(x => x.Firstname);
            Map(x => x.Lastname);
            Map(x => x.StatusType);
            Map(x => x.CreateDate);
            Map(x => x.Bank);
            Map(x => x.IBAN);
            Map(x => x.BranchCode);
            Map(x => x.AccountNumber);
            Map(x => x.Currency);

            Table("BankAccount");
        }
    }
}
