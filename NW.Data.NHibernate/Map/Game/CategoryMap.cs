using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Game
{
    public class CategoryMap : ClassMap<NW.Core.Entities.Category>
    {

        public CategoryMap()
        {
            Id(x => x.Id);
            Map(x => x.CompanyId);
            Map(x => x.CasinoCategoryTemplateId);
            Map(x => x.ParentCasinoCategoryId);
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.Active);
            Map(x => x.DisplayOrder);
            Map(x => x.FriendlyUrl);
            Map(x => x.ResourceKey);
            Map(x => x.CreateDate);

            References(x => x.Company).Column("CompanyId").ReadOnly();
            References(x => x.ParentCategory).Column("ParentCasinoCategoryId").ReadOnly();
            HasMany(x => x.GameCategories).KeyColumn("CategoryId").Inverse().Cascade.All();
            References(x => x.CategoryTemplate).Column("CasinoCategoryTemplateId").Not.LazyLoad().Fetch.Join().ReadOnly();

            Table("CasinoCategory");
        }
    }
}
