using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class CompanySetting : Entity<int>
    {
        public virtual int CompanyId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }
        public virtual int KeyGroupId { get; set; }
        public virtual bool Mode { get; set; }
    }
}
