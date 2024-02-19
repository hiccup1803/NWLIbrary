using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberPasscode : Entity<int>
    {
        public virtual int StatusType { get; set; }
        public virtual int MemberId { get; set; }
        public virtual Guid Guid { get; set; }
        public virtual string Device { get; set; }
        public virtual string Passcode { get; set; }
        public virtual DateTime CreateDate { get; set; } 
    }
}
