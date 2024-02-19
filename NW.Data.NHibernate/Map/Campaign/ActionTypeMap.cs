using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Campaign
{
    public class ActionTypeMap : ClassMap<NW.Core.Entities.Campaign.ActionType>
    {

        public ActionTypeMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);

            Table("ActionType");
        }
    }
}
