using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Campaign
{
    public class EventTypeMap : ClassMap<NW.Core.Entities.Campaign.EventType>
    {

        public EventTypeMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);

            Table("EventType");
        }
    }
}
