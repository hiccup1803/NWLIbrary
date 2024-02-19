using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class HeetpayRequest : Entity<int>
    {
        public virtual int StatusType { get; set; }
        public virtual int MemberId { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string IdentityNumber { get; set; }
        public virtual long Amount { get; set; }
        public virtual string ProviderRefId { get; set; }
        public virtual string CallbackData { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime? UpdateDate { get; set; }
        public virtual bool? WithBonus { get; set; }
        public virtual int? BonusId { get; set; }

    }
}
