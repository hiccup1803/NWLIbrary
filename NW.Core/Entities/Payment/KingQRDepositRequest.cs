using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class KingQRDepositRequest : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual int StatusType { get; set; }
        public virtual long Amount { get; set; }
        public virtual string Currency { get; set; }
        public virtual string ProviderRefId { get; set; }
        public virtual string PaymentMethod { get; set; }
        public virtual string SystemAdminUsername { get; set; }
        public virtual string CallbackData { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime? UpdateDate { get; set; }
    }
}