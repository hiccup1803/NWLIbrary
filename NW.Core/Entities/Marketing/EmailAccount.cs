using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Marketing
{
    public class EmailAccount : Entity<int>
    {
        public virtual int CompanyId { get; set; }
        public virtual string Email { get; set; }
        public virtual string SenderName { get; set; }
        public virtual int EmailType { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual bool IsDefault { get; set; }
    }
}
