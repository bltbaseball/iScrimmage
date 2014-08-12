using System.Linq;
using iScrimmage.Core.Data;
using iScrimmage.Core.Models;
using System.Collections.Generic;

namespace iScrimmage.Core.Data.Queries
{
    public class MemberByEmailQuery : TemplatedQuerySpec<MemberByEmailQuery, Member>
    {
        private string email;

        public MemberByEmailQuery(string email)
        {
            this.email = email;
        }

        public override Member Execute(IDataContext context)
        {
            return context.Query<Member>(Template, new { email = this.email }).FirstOrDefault();
        }
    }
}
