using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class ContentPage : Entity<int>
    {

        public virtual int PageId { get; set; }

        public virtual string Keywords { get; set; }

        public virtual string Description { get; set; }
        public virtual string PageName { get; set; }
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }
        //public virtual string LanguageCode { get; set; }

        public virtual int LanguageId { get; set; }
        public virtual int CompanyId { get; set; }

        public virtual DateTime CreatedDate { get; set; }
    }
}
