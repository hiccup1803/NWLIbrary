using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Marketing
{
    public class Campaign : Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual string UID { get; set; }
        public virtual string CreateDate { get; set; }
    }
}
