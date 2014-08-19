using System;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Authentication;
using System.Web.Http;
using System.Web.Security;
using iScrimmage.Core.Common;
using iScrimmage.Core.Data;
using iScrimmage.Core.Data.Queries;
using iScrimmage.Core.Data.Models;
using iScrimmage.Core.Security;
using NHibernate.Engine;
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
            try
            {
                if (credentials == null)
                {
                    throw new InvalidCredentialException("The provided credentials are not valid.");
                }

                if (String.IsNullOrEmpty(credentials.Email) || String.IsNullOrEmpty(credentials.Password))
                {
                    throw new InvalidCredentialException("The provided credentials are not valid.");
                }

                var query = new MemberByEmailQuery(credentials.Email);

                var member = Context.Execute(query);

                if (member == null)
                {
                    throw new InvalidCredentialException("The provided credentials are not valid.");
                }

                if (PasswordHash.ValidatePassword(credentials.Password, member.Password))
                {
                    // Don't return sensitive data.
                    member.Password = "";
                    member.ResetToken = "";
                    member.ResetTokenExpiresOn = null;
                    member.VerificationToken = "";

                    this.CurrentUser = UserSession.Initialize(member);
                    FormsAuthentication.SetAuthCookie(member.Email, false);

                    return Ok<dynamic>(new { Success = true, Member = member });
                }

                throw new InvalidCredentialException("The provided credentials are not valid.");
            }
            catch (Exception ex)
            {
                return Ok<dynamic>(new { Success = false, Error = ex.Message });
            }
        }

        [HttpPost]
        public IHttpActionResult Register([FromBody] dynamic data)
        {
            try
            {
                var newId = Guid.NewGuid();
                var hash = PasswordHash.CreateHash((string)data.Password);

                var member = new Member()
                {
                    Id = newId,
                    Email = data.Email,
                    Password = hash,
                    FirstName = data.FirstName,
                    LastName = data.LasttName,
                    VerificationToken = "",
                    ResetToken = "",
                    CreatedBy = newId,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedBy = newId,
                    ModifiedOn = DateTime.UtcNow
                };

                var contact = new Contact()
                {
                    Id = Guid.NewGuid(),
                    MemberId = newId,
                    Type = "Mobile",
                    PhoneNumber = data.Phone,
                    CreatedBy = newId,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedBy = newId,
                    ModifiedOn = DateTime.UtcNow
                };

                Context.BeginTransaction();

                Context.Insert<Member>(member);
                Context.Insert<Contact>(contact);

                // TODO: Save the role

                // Create transition data
                Transition.Membership.CreateUser(member, (string)data.Role, contact);

                Context.CommitTransaction();

                // Don't return sensitive data.
                member.Password = "";
                member.ResetToken = "";
                member.ResetTokenExpiresOn = null;
                member.VerificationToken = "";

                return Ok<dynamic>(new { Success = true, Member = member });
            }
            catch (Exception ex)
            {
                Context.RollbackTransaction(); 
                
                return Ok<dynamic>(new { Success = false, Error = ex.Message });
            }
        }

        [HttpPost]
        public IHttpActionResult CheckEmailAvailability([FromBody] Member data)
        {
            try
            {
                if (String.IsNullOrEmpty(data.Email))
                {
                    throw new ArgumentNullException("email", "The 'email' argumant is missing.");
                }

                var query = new MemberByEmailQuery(data.Email);

                var member = Context.Execute(query);

                if (member == null)
                {
                    return Ok<dynamic>(new {Success = true, Available = true});
                }
                else
                {
                    return Ok<dynamic>(new { Success = true, Available = false });
                }
            }
            catch (Exception ex)
            {
                return Ok<dynamic>(new { Success = false, Error = ex.Message });
            }
        }

        [HttpPost]
        public IHttpActionResult ResetPassword([FromBody] Member data)
        {
            try
            {
                var query = new MemberByResetToken(data.ResetToken, DateTime.UtcNow);
                var member = Context.Execute(query);

                if (member == null)
                {
                    return Ok<dynamic>(new { Success = false, Error = "This reset link is invalid or expired." });
                }

                member.Password = PasswordHash.CreateHash(data.Password);
                member.ResetToken = "";
                member.ModifiedBy = member.Id;
                member.ModifiedOn = DateTime.UtcNow;

                Context.Update<Member>(member.Id, member);

                return Ok<dynamic>(new { Success = true });
            }
            catch (Exception ex)
            {
                return Ok<dynamic>(new { Success = false, Error = ex.Message });
            }
        }

        [HttpPost]
        public IHttpActionResult SendResetLink([FromBody] Member credentials)
        {
            try
            {
                var query = new MemberByEmailQuery(credentials.Email);

                var member = Context.Execute(query);

                if (member == null)
                {
                    return Ok<dynamic>(new {Success = false, Error = "The provided email address could not be found."});
                }

                member.ResetToken = Guid.NewGuid().ToString().Replace("-", "");
                member.ResetTokenExpiresOn = DateTime.UtcNow.AddHours(24);
                member.ModifiedBy = Guid.Empty;
                member.ModifiedOn = DateTime.UtcNow;

                Context.Update<Member>(member.Id, member);

                EmailNotification.ResetPassword(member);

                return Ok<dynamic>(new {Success = true});
            }
            catch (Exception ex)
            {
                return Ok<dynamic>(new {Success = false, Error = ex.Message});
            }
        }
    }
}