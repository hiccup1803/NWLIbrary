using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class IPLoginLog : ClassMap<NW.Core.Entities.IPLoginLog>
    {
        public IPLoginLog()
        {
            Id(x => x.Id);
            Map(x => x.IP);
            Map(x => x.CreateDate);
            Map(x => x.MemberId);


            Table("IPLoginLog");
        }
    }
}
