using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class Promotion : Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual int CountryId { get; set; }
        public virtual int OrderNumber { get; set; }
        public virtual string Title { get; set; }
        public virtual string Summary { get; set; }

        public virtual string Terms { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool IsVipPromo { get; set; }

        public virtual string ThumbPicturePath { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime ExpireDate { get; set; }
        public virtual DateTime CreateDate { get; set; }

        public virtual int PromotionType { get; set; }
        public virtual string UsernameList { get; set; }

        public virtual string DataFilterCat { get; set; }
    }
}
