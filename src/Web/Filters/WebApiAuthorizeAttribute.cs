using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Autofac;
using iScrimmage.Core.Common;

namespace Web.Filters
{
    public class WebApiAuthorizeAttribute : AuthorizeAttribute
    {
        public IUserSessionProvider UserProvider { get; set; }
        public string[] AuthorizedRoles { get; set; }

        public WebApiAuthorizeAttribute() : this(Ioc.Current.Resolve<IUserSessionProvider>())
        {
            //UserProvider = ;
        }

        public WebApiAuthorizeAttribute(IUserSessionProvider userSessionProvider)
        {
            UserProvider = userSessionProvider;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var user = UserProvider.GetCurrentUser();

            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            if (user == null)
            {
                throw new AuthenticationException("User is not authenticated");
            }

            if (AuthorizedRoles != null && !AuthorizedRoles.Contains(user.Role))
            {
                throw new UnauthorizedAccessException("The current user is not authorized to access this service.");
            }
        }
    }
}