using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class CreditCardResponse : Entity<int> 
    {
        public virtual int CCRequestId { get; set; }
    
        public virtual Int64 Amount { get; set; }
        public virtual string Response { get; set; }
        public virtual bool BalanceUpdated { get; set; }

        public virtual Int64 BalanceRefId { get; set; }
      
        public virtual DateTime CreateDate { get; set; }
    }
}
