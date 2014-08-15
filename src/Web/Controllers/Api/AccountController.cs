using System;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using System.Web.Security;
using Google.GData.Client;
using iScrimmage.Core.Common;
using iScrimmage.Core.Data;
using iScrimmage.Core.Data.Queries;
using iScrimmage.Core.Data.Models;
using iScrimmage.Core.Security;
using Web.Helpers;

namespace Web.Controllers.Api
{
    public class AccountController : BaseApiController
    {
        public AccountController(IDataContext context) : base(context)
        {
        }

        [HttpGet]
        public IHttpActionResult Logout()
        {
            this.CurrentUser = null;
            FormsAuthentication.SignOut();

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Login([FromBody] Member credentials)
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

                this.CurrentUser = UserSession.Initialize(member);
                FormsAuthentication.SetAuthCookie(member.Email, false);

                return Ok<Member>(member);
            }

            throw new InvalidCredentialsException();
        }

        [HttpPost]
        public IHttpActionResult Register([FromBody] Member member)
        {
            return Ok<Member>(member);
        }

        [HttpPost]
        public IHttpActionResult SendResetLink([FromBody] Member credentials)
        {
            var query = new MemberByEmailQuery(credentials.Email);

            var member = Context.Execute(query);

            if (member == null)
            {
                return Ok<dynamic>(new {Error = "The provided email address could not be found."});
            }

            member.ResetToken = Guid.NewGuid().ToString().Replace("-", "");
            member.ResetTokenExpiresOn = DateTime.UtcNow.AddHours(24);
            member.ModifiedBy = Guid.Empty;
            member.ModifiedOn = DateTime.UtcNow;

            Context.Update<Member>(member.Id, member);

            EmailNotification.ResetPassword(member);

            return Ok<Member>(credentials);
        }
    }
}