using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Web;
using System.Web.Http.Filters;
using Newtonsoft.Json.Linq;

namespace Web.Filters
{
    public class WebApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var exceptionType = context.Exception.GetType();

            var statusCode = HttpStatusCode.InternalServerError;
            var content = JObject.FromObject(new { ExceptionMessage = context.Exception.Message });

            if (exceptionType == typeof(AuthenticationException))
            {
                statusCode = HttpStatusCode.Unauthorized;
            }

            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                statusCode = HttpStatusCode.Unauthorized;
            }

            context.Response = new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent(content.ToString())
            };
        }
    }
}