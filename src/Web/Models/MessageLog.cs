using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class MessageLog
    {
        public virtual int Id { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }
        public virtual int RecipientCount { get; set; }
        public virtual string Recipients { get; set; }
        public virtual DateTime SentOn { get; set; }
        public virtual User SentBy { get; set; }

        public static IList<MessageLog> GetMessageLogsForUser(User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Web.Models.MessageLog>().Where(l=>l.SentBy == user).OrderBy(l=>l.SentOn).Desc.List();
        }
    }
}