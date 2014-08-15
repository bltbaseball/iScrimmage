using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Autofac;
using DotNetOpenAuth.AspNet;
using iScrimmage.Core.Common;
using iScrimmage.Core.Data;
using iScrimmage.Core.Data.Models;
using iScrimmage.Core.Data.Queries;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Web.Filters;
using Web.Models;
using System.Text;
using System.Configuration;

namespace Web.Controllers
{
    [Authorize]
    //[InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        public ActionResult Manage()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            if (user == null)
                return View();
            var model = new LocalPasswordModel();

            switch (user.Role)
            {
                case UserRole.Coach:
                    var coach = Coach.GetCoachForUser(user);
                    model.NotificationEmail = coach.Email;
                    break;
                case UserRole.Manager:
                    var manager = Manager.GetManagerForUser(user);
                    model.NotificationEmail = manager.Email;
                    break;
                case UserRole.Player:
                    var player = Player.GetPlayerForUser(user);
                    model.NotificationEmail = player.Email;
                    model.IsLookingForTeam = player.IsLookingForTeam;
                    break;
                case UserRole.Guardian:
                    var guardian = Guardian.GetGuardianForUser(user);
                    model.NotificationEmail = guardian.Email;
                    break;
                case UserRole.Umpire:
                    var umpire = Umpire.GetUmpireForUser(user);
                    model.NotificationEmail = umpire.Email;
                    break;
                default:
                    model.NotificationEmail = user.Email;
                    break;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var session = MvcApplication.SessionFactory.GetCurrentSession();
                    
            if (user == null)
                return View();

            using (var tx = session.BeginTransaction())
            {

                switch (user.Role)
                {
                    case UserRole.Coach:
                        var coach = Coach.GetCoachForUser(user);
                        coach.Email = model.NotificationEmail;
                        session.Update(coach);
                        break;
                    case UserRole.Manager:
                        var manager = Manager.GetManagerForUser(user);
                        manager.Email = model.NotificationEmail;
                        session.Update(manager);
                        break;
                    case UserRole.Player:
                        var player = Player.GetPlayerForUser(user);
                        player.Email = model.NotificationEmail;
                        player.IsLookingForTeam = model.IsLookingForTeam;
                        session.Update(player);
                        break;
                    case UserRole.Guardian:
                        var guardian = Guardian.GetGuardianForUser(user);
                        guardian.Email = model.NotificationEmail;
                        session.Update(guardian);
                        break;
                    case UserRole.Umpire:
                        var umpire = Umpire.GetUmpireForUser(user);
                        umpire.Email = model.NotificationEmail;
                        session.Update(umpire);
                        break;
                }
                user.IsEmailConfirmed = true;
                session.Update(user);
                tx.Commit();
            }
            return RedirectToAction("Dashboard", "Home");
        }
  
        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return Redirect("https://www.google.com/accounts/Logout?continue=https://appengine.google.com/_ah/logout?continue=http://" + 
                System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port);
            //return RedirectToAction("Index", "Home");
            //return RedirectToAction("Index", "Home");
        }

        // We don't need a separate registration form. It's automatically handled by the Login/OAuth authentication.
        // After a user logs in for the first time, they should be redirected to their invitation page to fill out their
        // details and have the User account associated with a Manager, Coach, Player, Guardian or Umpire.

        public ActionResult Unauthorized()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalSignup(string provider, string returnUrl)
        {
            var url = "https://accounts.google.com/Signup";
            return Redirect(url);
        }


        [AllowAnonymous]
        public ActionResult ExternalLogin()
        {
            var provider = "google";
            var returnUrl = "/Home/Dashboard";
            var url = "https://www.google.com/accounts/Logout?continue=https://appengine.google.com/_ah/logout?continue=" +
                Server.UrlEncode(Server.UrlEncode("http://" +
                System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port
                + "/Account/ExternalLogin2?provider=" + provider + "&returnUrl=" + returnUrl));
            return Redirect(url);
        }
        
        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            var url = "https://www.google.com/accounts/Logout?continue=https://appengine.google.com/_ah/logout?continue=" +
                Server.UrlEncode(Server.UrlEncode("http://" + 
                System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port 
                + "/Account/ExternalLogin2?provider=" + provider + "&returnUrl=" + returnUrl ));
            return Redirect(url);
        }

        //
        // GET: /Account/ExternalLogin2

        [AllowAnonymous]
        public ActionResult ExternalLogin2(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            //try
            //{
                AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
                var test = OAuthWebSecurity.GetOAuthClientData(result.Provider);
                if (!result.IsSuccessful)
                {
                    return RedirectToAction("ExternalLoginFailure");
                }

                var validUser = Web.Models.User.GetUserViaOAuthCredentials(result.Provider, result.ProviderUserId);
                if (validUser != null)
                {
                    Web.Models.User.ValidateUserRole(validUser);

                    // log the user in
                    FormsAuthentication.SetAuthCookie(validUser.Email, false);
                    if(validUser.IsEmailConfirmed == false && validUser.Role != UserRole.Administrator)
                        return RedirectToAction("Manage", "Account");
                    return RedirectToLocal(returnUrl);
                }

                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                string email = null;
                string firstName = null;
                string lastName = null;
                if (result.ExtraData.ContainsKey("email"))
                {
                    email = result.ExtraData["email"];
                }
                if (result.ExtraData.ContainsKey("firstName"))
                {
                    firstName = result.ExtraData["firstName"];
                }
                if (result.ExtraData.ContainsKey("lastName"))
                {
                    lastName = result.ExtraData["lastName"];
                }

                // if a user already exists with this email, see if they do not have an oauth provider yet, and if not, link them to this user
                var existingUser = Web.Models.User.GetUserByEmail(email);
                if ((existingUser != null) && (existingUser.OAuthMemberships.Count == 0))
                {
                    Web.Models.User.LinkGoogleOAuthToUser(existingUser, result.ProviderUserId, firstName, lastName);
                    Web.Models.User.ValidateUserRole(existingUser);
                    FormsAuthentication.SetAuthCookie(existingUser.Email, false);
                    if (existingUser.IsEmailConfirmed == false && existingUser.Role != UserRole.Administrator)
                        return RedirectToAction("Manage", "Account");
                    return RedirectToLocal(returnUrl);
                }

                if (existingUser != null)
                {
                    // somehow the OAuth changed.. why does this happen?
                    var currentId = existingUser.OAuthMemberships[0];
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException(string.Format("OAuth ID changed for user: {0} from {1} to {2}",  email, currentId.ProviderUserId, result.ProviderUserId)));
                    existingUser.OAuthMemberships[0].ProviderUserId = result.ProviderUserId;
                    var session = MvcApplication.SessionFactory.GetCurrentSession();
                    session.Update(existingUser);
                    Web.Models.User.ValidateUserRole(existingUser);
                    FormsAuthentication.SetAuthCookie(existingUser.Email, false);
                    if (existingUser.IsEmailConfirmed == false && existingUser.Role != UserRole.Administrator)
                        return RedirectToAction("Manage", "Account");
                    return RedirectToLocal(returnUrl);
                }



                // user does not already exist, register them with the site
                var user = Web.Models.User.CreateUserFromGoogleOAuth(result.ProviderUserId, email, firstName, lastName);
                if (user != null)
                {
                    var session = MvcApplication.SessionFactory.GetCurrentSession();
                    using (var tx = session.BeginTransaction())
                    {
                        var player = Player.GetPlayerForEmail(email);
                        if (player != null)
                        {
                            player.User = user;
                            user.Role = UserRole.Player;
                            session.Update(user);
                            ControllerContext.HttpContext.Cache.Remove("User-" + email);
                            session.Save(player);
                            tx.Commit();
                            Web.Helpers.EmailNotification.NewPlayer(player);
                        }
                        var guardian = Guardian.GetGuardianForEmail(email);
                        if (guardian != null)
                        {
                            guardian.User = user;
                            user.Role = UserRole.Guardian;
                            session.Update(user);
                            ControllerContext.HttpContext.Cache.Remove("User-" + email);
                            session.Save(guardian);
                            tx.Commit();
                            Web.Helpers.EmailNotification.NewGuardian(guardian);
                        }
                        var coach = Coach.GetCoachForEmail(email);
                        if (coach != null)
                        {
                            coach.User = user;
                            user.Role = UserRole.Coach;
                            session.Update(user);
                            ControllerContext.HttpContext.Cache.Remove("User-" + email);
                            session.Save(coach);
                            tx.Commit();
                            Web.Helpers.EmailNotification.NewCoach(coach);
                        }
                        var umpire = Umpire.GetUmpireForEmail(email);
                        if (umpire != null)
                        {
                            umpire.User = user;
                            user.Role = UserRole.Umpire;
                            session.Update(user);
                            ControllerContext.HttpContext.Cache.Remove("User-" + email);
                            session.Save(umpire);
                            tx.Commit();
                        }
                    }

                    Web.Models.User.ValidateUserRole(user);
                    FormsAuthentication.SetAuthCookie(user.Email, false);
                    if (user.IsEmailConfirmed == false && user.Role != UserRole.Administrator)
                        return RedirectToAction("Manage", "Account");
                    return RedirectToLocal(returnUrl);
                }
            //}
            //catch (Exception e)
            //{
            //    Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            //    throw;
            //}
            return RedirectToAction("ExternalLoginFailure");
        }


        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                ViewData.Model = null;
            }
            else
            {
                var context = Ioc.Current.Resolve<IDataContext>();
                var query = new MemberByResetToken(id, DateTime.UtcNow);
                var member = context.Execute(query);

                if (member == null)
                {
                    // Create member with empty id to indicate no member was found by the token.
                    member = new Member();
                    member.Id = Guid.Empty;
                }

                // Don't return sensitive data.
                member.Password = "";
                member.ResetTokenExpiresOn = null;
                member.VerificationToken = "";

                // Will need the reset token
                member.ResetToken = id;

                ViewData.Model = member;
            }

            return View();
        }

        
        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
