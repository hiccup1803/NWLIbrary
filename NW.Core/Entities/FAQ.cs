using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class FAQ : Entity<int>
    {
        public virtual string Title { get; set; }
        public virtual string BodyContent { get; set; }
        public virtual int CategoryId { get; set; }
        public virtual int LanguageId { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual int ItemOrder { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual byte Status { get; set; }

        public FAQ()
        {
            Status = 1;
            this.CreateDate = DateTime.UtcNow;
        }
    }
}
