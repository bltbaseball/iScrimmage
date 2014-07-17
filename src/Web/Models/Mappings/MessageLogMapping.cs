using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class MessageLogMapping : ClassMap<MessageLog>
    {
        public MessageLogMapping()
        {
            Table("MessageLogs");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.Body);
            Map(c => c.RecipientCount);
            Map(c => c.Recipients);
            Map(c => c.SentOn);
            Map(c => c.Subject);
            References(c => c.SentBy);
        }
    }
}