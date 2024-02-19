using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class PayfixTransferPayfixAccount : Entity<int>
    {
        public virtual int StatusType { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string NameSurname { get; set; }
        public virtual string NameSurnameMasked { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string BlaclistedUsernameList { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual int ProviderId { get; set; }
        public virtual IList<Level> Levels { get; set; }
        public virtual Company Company { get; set; }
        public virtual Provider Provider { get; set; }

    }
}
