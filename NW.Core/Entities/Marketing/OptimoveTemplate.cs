using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Marketing
{
    public class OptimoveTemplate : Entity<int>
    {
        public virtual int TemplateType { get; set; }
        public virtual int StatusType { get; set; }
        public virtual string Name { get; set; }
        public virtual string Content { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime? UpdateDate { get; set; }
    }
}
