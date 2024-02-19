using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class ReyPayCMTRequestMap : ClassMap<NW.Core.Entities.Payment.ReyPayCMTRequest>
    {
        public ReyPayCMTRequestMap()
        {
            Id(x => x.Id);
            Map(x => x.MemberId);
            Map(x => x.StatusType);
            Map(x => x.CreateDate);
            Map(x => x.Amount);
            Map(x => x.ProcessId);
            Map(x => x.ReferenceCode);
            Map(x => x.ResultData);
            Table("ReyPayCMTRequest");
        }
    }
}
