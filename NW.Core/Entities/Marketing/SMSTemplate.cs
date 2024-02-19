using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Marketing
{
    public class SmsTemplate:Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual string Content { get; set; }
        public virtual DateTime CreateDate { get; set; }
        
    }
}
