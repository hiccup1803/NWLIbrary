using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class Tag : Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual string Slug { get; set; }
        public virtual DateTime CreateDate { get; set; }
    }
}
