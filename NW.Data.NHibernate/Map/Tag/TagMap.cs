using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Tag
{
    public class TagMap : ClassMap<NW.Core.Entities.Tag>
    {

        public TagMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Slug);
            Map(x => x.CreateDate);

            Table("Tag");
        }
    }
}
