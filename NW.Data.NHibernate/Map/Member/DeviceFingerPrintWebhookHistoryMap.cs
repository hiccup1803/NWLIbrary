using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class DeviceFingerPrintWebhookHistoryMap : ClassMap<DeviceFingerPrintWebhookHistory>
    {
        public DeviceFingerPrintWebhookHistoryMap()
        {
            Id(l => l.Id);
            Map(l => l.Hash);
            Map(l => l.CreateDate);
            Map(l => l.Data);
            Map(l => l.BotProbability);
            Map(l => l.RequestId);
            Map(l => l.IP);
        }
    }
}
