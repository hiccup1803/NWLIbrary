using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberDeviceFingerPrintHistory : Entity<int>
    {
        public virtual Member Member { get; set; }
        public virtual int MemberId { get; set; } 
        public virtual string Hash { get; set; }
        public virtual int StatusType { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual string Geolocation { get; set; }
        public virtual string IP { get; set; }
        public virtual bool? IsLoggedIn { get; set; }

    }
}
