using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Campaign
{
    public class EventType : Entity<int>
    {
        public virtual string Name { get; set; }
    }
}
