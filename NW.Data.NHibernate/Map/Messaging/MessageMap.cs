using FluentNHibernate.Mapping;
using NW.Core;
using NW.Core.Entities;
using NW.Core.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Messaging
{
    public class MessageMap : ClassMap<Message>
    {
        public MessageMap()
        {
            Id(x => x.Id);
            Map(x => x.MessageTopicId);
            Map(x => x.MemberId);
            Map(x => x.MessageType);
            Map(x => x.Title);
            Map(x => x.Content);
            Map(x => x.CreateDate);
            Map(x => x.ReadDate);
            Map(x => x.CreatedBy);
            Map(x => x.StatusType);
            Map(x => x.StartDate);
            Map(x => x.EndDate);


            References(x => x.Member).Column("MemberId").ReadOnly();
            References(x => x.MessageTopic).Column("MessageTopicId").ReadOnly();

            Table("Message");

        }

    }
}
