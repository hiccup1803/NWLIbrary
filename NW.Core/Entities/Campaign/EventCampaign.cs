using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Campaign
{
    public class EventCampaign : Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual int EventTypeId { get; set; }
        public virtual int ActionTypeId { get; set; }
        public virtual long Amount { get; set; }
        public virtual int Percentage { get; set; }
        public virtual long MaxAmount { get; set; }
        public virtual int MaxUsageCount { get; set; }
        public virtual string UsernameList { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual int CampaignUserRestrictionType { get; set; }
        public virtual int EventCampaignPrizeType { get; set; }
        public virtual bool IsVip { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int StatusType { get; set; }
        public virtual EventType EventType { get; set; }
        public virtual ActionType ActionType { get; set; }
        public virtual Company Company { get; set; }

    }
}
