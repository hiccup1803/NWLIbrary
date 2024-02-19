using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map
{
    public class CustomStuffMap: ClassMap<NW.Core.Entities.CustomStuff> {

        public CustomStuffMap()
        {
			Id(x => x.Id);
			Map(x => x.DataKey);
            Map(x => x.Data);
            Map(x => x.ExpiryDate);
        }
    }
}
