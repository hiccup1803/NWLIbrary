using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class IPBlacklistMap : ClassMap<NW.Core.Entities.IPBlacklist>
    {
        public IPBlacklistMap()
        {
            Id(x => x.Id);
            Map(x => x.IP);
            Map(x => x.CreateDate);
            Map(x => x.BlockTo);


            Table("IPBlacklist");
        }
    }
}
