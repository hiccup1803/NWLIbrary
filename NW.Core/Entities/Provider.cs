using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class Provider : Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual int ProviderTypeId { get; set; }
        public virtual int VoltronProviderId { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string SystemName { get; set; }
        public virtual ProviderType ProviderType { get; set; }
        public virtual IList<ProviderSetting> ProviderSettings { get; set; }
    }
}
