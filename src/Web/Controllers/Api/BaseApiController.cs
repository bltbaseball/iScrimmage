using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using iScrimmage.Core.Common;
using iScrimmage.Core.Data;
using Web.Filters;

namespace Web.Controllers.Api
{
    public abstract class BaseApiController : ApiController
    {
        protected IDataContext Context;

        public IUserSession CurrentUser { get; set; }

        protected BaseApiController(IDataContext context)
        {
           CurrentUser = UserSession.Current;

           Context = context;
        }
    }
}