using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class PayKasaTransaction : Entity<int> 
    {
        public virtual int MemberId { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual string Request { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int Status { get; set; }
        public virtual string Response { get; set; }
        public virtual Int64 WalletTransactionRefId { get; set; }
        public virtual string ProviderRef { get; set; }
 
    }
}