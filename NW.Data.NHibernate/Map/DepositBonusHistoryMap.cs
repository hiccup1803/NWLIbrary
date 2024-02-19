using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map
{
    public class DepositBonusHistoryMap : ClassMap<NW.Core.Entities.DepositBonusHistory>
    {

        public DepositBonusHistoryMap()
        {
            Id(x => x.Id);
            Map(x => x.MemberId);
            Map(x => x.VoltronTransactionTypeId);
            Map(x => x.PaymentProviderId);
            Map(x => x.Amount);
            Map(x => x.CreateDate);
        }
    }
}
