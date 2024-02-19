using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Model.Game
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int ParentCasinoCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public int DisplayOrder { get; set; }
        public string FriendlyUrl { get; set; }
        public string ResourceKey { get; set; }
        public DateTime CreateDate { get; set; }



        public CategoryTemplateModel CategoryTemplate { get; set; }
    }
}
