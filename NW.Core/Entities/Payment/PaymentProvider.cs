using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class PaymentProvider : Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual bool IsOpen { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual long? MaxAmount { get; set; }
        public virtual long? MinAmount { get; set; }
        public virtual string Currency { get; set; }
    }
}
