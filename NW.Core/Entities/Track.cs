using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class Track : Entity<Guid>
    {
        public virtual int ActionType { get; set; }
        public virtual string UserIdentity { get; set; }
        public virtual string SessionId { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string Value { get; set; }
        public virtual string ExtraFields { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual string Ip { get; set; }
    }
}
