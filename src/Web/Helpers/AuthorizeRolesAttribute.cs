using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Helpers
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        private readonly UserRole[] roles;
        public AuthorizeRolesAttribute(params UserRole[] roles)
        {
            this.roles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
                return false;

            var user = Web.Models.User.GetUserByEmail(httpContext.User.Identity.Name);
            if (user == null)
                return false;

            if (this.roles.Length == 0)
                return false;

            if (!this.roles.Contains(user.Role))
                return false;

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new System.Web.Mvc.HttpStatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}