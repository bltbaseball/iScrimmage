using System;
using System.Linq;
using iScrimmage.Core.Data.Models;
using System.Collections.Generic;

namespace iScrimmage.Core.Data.Queries
{
    public class MemberByResetToken : TemplatedQuerySpec<MemberByResetToken, Member>
    {
        private string token;
        private DateTime checkTime;

        public MemberByResetToken(string token, DateTime checkTime)
        {
            this.token = token;
            this.checkTime = checkTime;
        }

        public override Member Execute(IDataContext context)
        {
            return context.Query<Member>(Template, new { token = this.token, checkTime = this.checkTime }).FirstOrDefault();
        }
    }
}
