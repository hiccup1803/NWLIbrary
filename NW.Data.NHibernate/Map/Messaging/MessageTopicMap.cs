using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Messaging
{
    public class MessageTopicMap : ClassMap<MessageTopic>
    {
        public MessageTopicMap()
        {
            Id(x => x.Id);
            Map(x => x.MessageType);
            Map(x => x.Title);
            Map(x => x.Content);
            Map(x => x.CreateDate);
            Map(x => x.CreatedBy);
            Map(x => x.StatusType);
            Map(x => x.SystemName);
            Map(x => x.CompanyId);

            Table("MessageTopic");

        }

    }
}
