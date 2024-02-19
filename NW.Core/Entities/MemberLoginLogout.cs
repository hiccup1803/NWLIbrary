using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberLoginLogout : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual DateTime LoginDate { get; set; }
        public virtual DateTime? LogoutDate { get; set; }
    }
}
