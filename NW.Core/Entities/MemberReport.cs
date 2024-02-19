using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberReport : Member
    {
        public virtual int? VoltronMemberId { get; set; }
        public virtual bool EmailVerified { get; set; }
    }
}
