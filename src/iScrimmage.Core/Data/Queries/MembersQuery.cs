using iScrimmage.Core.Data;
using iScrimmage.Core.Data.Models;
using System.Collections.Generic;

namespace iScrimmage.Core.Queries
{
    public class MembersQuery : TemplatedQuerySpec<MembersQuery, IEnumerable<Member>>
    {
        public MembersQuery()
        {
        }

        public override IEnumerable<Member> Execute(IDataContext context)
        {
            return context.Query<Member>(Template, new { });
        }
    }
}
