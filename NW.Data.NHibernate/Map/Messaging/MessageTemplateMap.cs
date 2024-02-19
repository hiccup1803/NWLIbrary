using FluentNHibernate.Mapping;
using NW.Core;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Messaging
{
    public class MessageTemplateMap : ClassMap<MessageTemplate>
    {

        public MessageTemplateMap()
        {
            Id(x => x.Id);
            Map(x => x.MessageType);
            Map(x => x.Title);
            Map(x => x.Content);
            Map(x => x.CreateDate);
            Map(x => x.StatusType);
            Map(x => x.CompanyId);

            Table("MessageTemplate");

        }
    }
}
