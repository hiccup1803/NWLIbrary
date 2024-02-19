using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class CompanyBackOfficeDomain : Entity<int>
    {
        public virtual string Domain { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual int VoltronCompanyId { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual bool IsLive { get; set; }
    }
}
