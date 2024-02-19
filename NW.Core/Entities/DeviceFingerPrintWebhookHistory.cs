using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class DeviceFingerPrintWebhookHistory : Entity<int>
    {
        public virtual string Hash { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string Data { get; set; }
        public virtual string RequestId { get; set; }
        public virtual bool? BotProbability { get; set; }
        public virtual string IP { get; set; }
    }
}
