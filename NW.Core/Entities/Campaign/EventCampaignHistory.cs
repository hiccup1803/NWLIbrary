using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Campaign
{
    public class EventCampaignHistory : Entity<int>
    {
        public virtual int EventCampaignId { get; set; }
        public virtual int MemberId { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual EventCampaign EventCampaign { get; set; }
        public virtual Member Member { get; set; }

    }
}
