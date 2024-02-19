using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Campaign
{
    public class EventCampaignMap : ClassMap<NW.Core.Entities.Campaign.EventCampaign>
    {
        public EventCampaignMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.EventTypeId);
            Map(x => x.ActionTypeId);
            Map(x => x.Amount);
            Map(x => x.Percentage);
            Map(x => x.MaxAmount);
            Map(x => x.MaxUsageCount);
            Map(x => x.UsernameList).Length(4001);
            Map(x => x.CompanyId);
            Map(x => x.CampaignUserRestrictionType);
            Map(x => x.EventCampaignPrizeType);
            Map(x => x.IsVip);
            Map(x => x.StartDate);
            Map(x => x.EndDate);
            Map(x => x.CreateDate);
            Map(x => x.StatusType);

            References(x => x.EventType).Column("EventTypeId").ReadOnly();
            References(x => x.ActionType).Column("ActionTypeId").ReadOnly();
            References(x => x.Company).Column("CompanyId").ReadOnly();

            Table("EventCampaign");
        }
    }
}