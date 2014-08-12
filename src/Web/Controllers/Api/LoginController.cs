using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Google.GData.Client;
using iScrimmage.Core.Common;
using iScrimmage.Core.Data;
using iScrimmage.Core.Data.Queries;
using iScrimmage.Core.Models;
using iScrimmage.Core.Security;

namespace Web.Controllers.Api
{
    public class LoginController : BaseApiController
    {
        public LoginController(IDataContext context) : base(context)
        {
        }

        public IHttpActionResult Post([FromBody] Member credentials)
        {
            if (credentials == null)
            {
                throw new InvalidCredentialsException();
            }

            if (String.IsNullOrEmpty(credentials.Email) || String.IsNullOrEmpty(credentials.Password))
            {
                throw new InvalidCredentialsException();
            }

            var query = new MemberByEmailQuery(credentials.Email);

            var member = Context.Execute(query);

            if (member == null)
            {
                throw new InvalidCredentialsException();
            }

            if (PasswordHash.ValidatePassword(credentials.Password, member.Password))
            {
                // Clear password for security purposes.
                member.Password = "";

                // Set the current user of the session provider
                this.CurrentUser = UserSession.Initialize(member);

                return Ok<Member>(member);
            }

            throw new InvalidCredentialsException();
        }
    }
}