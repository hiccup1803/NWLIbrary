using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class Language : Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual string Culture { get; set; }
        public virtual string IVRCode { get; set; }
        public virtual byte Status { get; set; }
    }
}
