using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberNotification : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual bool DesktopPushEnabled { get; set; }
        public virtual bool MobilePushEnabled { get; set; }
        public virtual bool FacebookPushEnabled { get; set; }
        public virtual string Endpoint { get; set; }
        public virtual string SubscriptionId { get; set; }
        public virtual bool IsGCM { get; set; }
        public virtual DateTime CreateDate { get; set; }
    }
}