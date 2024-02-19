using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberPhoneVerification : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual string Code { get; set; }
        public virtual string Phone { get; set; }
        public virtual int SentCount { get; set; }
        public virtual bool Verified { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime VerifyDate { get; set; }
    }
}
