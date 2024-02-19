using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class RejectedBin : Entity<int>
    {
        
        public virtual string FilteredCardNo { get; set; }
        public virtual DateTime CreatedDate { get; set; }
    }
}
