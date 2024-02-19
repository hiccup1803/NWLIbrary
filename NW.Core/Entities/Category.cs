using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class Category : Entity<int>
    {
        public virtual int CompanyId { get; set; }
        public virtual int? CasinoCategoryTemplateId { get; set; }
        public virtual int ParentCasinoCategoryId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Active { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual string FriendlyUrl { get; set; }
        public virtual string ResourceKey { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual Company Company { get; set; }        
        public virtual CategoryTemplate CategoryTemplate { get; set; }
        public virtual IEnumerable<GameCategory> GameCategories { get; set; }
        public virtual Category ParentCategory { get; set; }
    }
}
