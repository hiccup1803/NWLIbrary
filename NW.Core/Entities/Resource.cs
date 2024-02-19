using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class Resource : Entity<int>
    {
        public virtual int DomainId { get; set; }
        public virtual string VirtualPath { get; set; }
        public virtual string ClassName { get; set; }
        public virtual int LanguageId { get; set; }
        public virtual string Culture { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual string ResourceValue { get; set; }

    }
}
