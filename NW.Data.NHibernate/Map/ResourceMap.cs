using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map
{
    public class ResourceMap : ClassMap<NW.Core.Entities.Resource> 
    {
        ResourceMap()
        {
            Id(x => x.Id).Column("ResourceId");
            Map(x => x.DomainId);
            Map(x => x.VirtualPath);
            Map(x => x.ClassName);
            Map(x => x.LanguageId);
            Map(x => x.Culture);
            Map(x => x.ResourceName);
            Map(x => x.ResourceValue);

            Table("CMS_Resource");
        }
    }
}
