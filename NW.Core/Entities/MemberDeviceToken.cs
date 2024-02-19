using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberDeviceToken : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual string UserAgent { get; set; }
        public virtual string IP { get; set; }
        public virtual string CountryCode { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime LastUsedDate { get; set; }
    }
}