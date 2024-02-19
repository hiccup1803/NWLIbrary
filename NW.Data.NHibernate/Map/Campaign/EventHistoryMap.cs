using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Campaign
{
    public class EventHistoryMap : ClassMap<NW.Core.Entities.Campaign.EventHistory>
    {
        public EventHistoryMap()
        {
            Id(x => x.Id);
            Map(x => x.EventTypeId);
            Map(x => x.VoltronTransactionTypeId);
            Map(x => x.VoltronTransactionId);
            Map(x => x.MemberId);
            Map(x => x.CompanyId);
            Map(x => x.Amount);
            Map(x => x.Username);
            Map(x => x.CreateDate);
            Map(x => x.StatusType);

            References(x => x.EventType).Column("EventTypeId").ReadOnly();
            References(x => x.Member).Column("MemberId").ReadOnly();
            References(x => x.Company).Column("CompanyId").ReadOnly();

            Table("EventHistory");
        }
    }
}
