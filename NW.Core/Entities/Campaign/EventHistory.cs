using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Campaign
{
    public class EventHistory : Entity<int>
    {
        public virtual int EventTypeId { get; set; }
        public virtual int VoltronTransactionTypeId { get; set; }
        public virtual int VoltronTransactionId { get; set; }
        public virtual int MemberId { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual long Amount { get; set; }
        public virtual string Username { get; set; }
        public virtual int StatusType { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual EventType EventType { get; set; }
        public virtual Member Member { get; set; }
        public virtual Company Company { get; set; }

    }
}
