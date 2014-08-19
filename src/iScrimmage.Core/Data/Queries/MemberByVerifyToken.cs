
using System.Collections.Generic;
using System.Linq;
using iScrimmage.Core.Data.Models;

namespace iScrimmage.Core.Data.Queries
{
    public class MemberByVerifyToken : TemplatedQuerySpec<MemberByVerifyToken, Member>
    {
        private string token;

        public MemberByVerifyToken(string token)
        {
            this.token = token;
        }

        public override Member Execute(IDataContext context)
        {
            return context.Query<Member>(Template, new { token = this.token }).FirstOrDefault();
        }
    }
}
