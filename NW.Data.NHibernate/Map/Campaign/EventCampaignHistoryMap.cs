using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Campaign
{
    public class EventCampaignHistoryMap : ClassMap<NW.Core.Entities.Campaign.EventCampaignHistory>
    {
        public EventCampaignHistoryMap()
        {
            Id(x => x.Id);
            Map(x => x.EventCampaignId);
            Map(x => x.MemberId);
            Map(x => x.CreateDate);

            References(x => x.EventCampaign).Column("EventCampaignId").ReadOnly();
            References(x => x.Member).Column("MemberId").ReadOnly();

            Table("EventCampaignHistory");
        }
    }
}
