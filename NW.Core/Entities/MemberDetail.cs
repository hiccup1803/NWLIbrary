using System;
using System.Text;
using System.Collections.Generic;


namespace NW.Core.Entities{
    
    public class MemberDetail : Entity<int> 
    {
        public virtual Member Member { get; set; }
        public virtual int MemberId { get; set; }
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
    }
}
