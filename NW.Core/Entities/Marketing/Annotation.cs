using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Marketing
{
    public class Annotation:Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual bool ShowAlways { get; set; }
        public virtual DateTime ActivityDate { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string UrlToRedirect { get; set; }
        public virtual string UrlAlias { get; set; }
    }
}
