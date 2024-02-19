using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class ProviderSetting : Entity<int>
    {
        public virtual int ProviderId { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }
        public virtual bool IsProduction { get; set; }
        public virtual bool Mode { get; set; }
        public virtual Company Company { get; set; }
        public virtual Provider Provider { get; set; }
    }
}
