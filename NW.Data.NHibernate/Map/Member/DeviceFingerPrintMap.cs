using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class DeviceFingerPrintMap : ClassMap<DeviceFingerPrint>
    {
        public DeviceFingerPrintMap()
        {
            Id(l => l.Id);
            Map(l => l.Hash);
            Map(l => l.StatusType);
            Map(l => l.CreateDate);
            Map(l => l.UpdateDate);
        }
    }
}
