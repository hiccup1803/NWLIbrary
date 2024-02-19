using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class AffiliateType : Entity<int>
    {
        public virtual string Name { get; set; }
    }
}
