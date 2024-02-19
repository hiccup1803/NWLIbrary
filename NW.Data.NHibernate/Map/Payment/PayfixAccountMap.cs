using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class PayfixAccountMap : ClassMap<NW.Core.Entities.Payment.PayfixAccount>
    {
        public PayfixAccountMap()
        {
            Id(x => x.Id);
            Map(x => x.MemberId);
            Map(x => x.Firstname);
            Map(x => x.Lastname);
            Map(x => x.StatusType);
            Map(x => x.CreateDate);
            Map(x => x.AccountNumber);
            Map(x => x.Currency);

            References(m => m.Member).Column("MemberId").ReadOnly();

            Table("PayfixAccount");
        }
    }
}
