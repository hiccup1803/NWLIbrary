using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Game
{
    public class CategoryTemplateMap : ClassMap<NW.Core.Entities.CategoryTemplate>
    {

        public CategoryTemplateMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.StatusType);
            Map(x => x.SystemName);
            Map(x => x.CreateDate);


            Table("CasinoCategoryTemplate");
        }
    }
}
