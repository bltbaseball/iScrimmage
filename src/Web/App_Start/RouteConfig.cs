using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Web.Models;
using DoddleReport.Web;

namespace Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("elmah.axd");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Invitation", "Invitation/{action}/{id}/{token}", new { controller = "Invitation", token = "", id = "" });

            routes.MapRoute("Static", "Static/{page}", new { controller = "Static", action = "StaticView", page = "" });
            routes.MapRoute("Tournament", "Home/Tournament/{id}/{teamId}", new { controller = "Home", action = "League", id = "", teamId = UrlParameter.Optional });
            routes.MapRoute("League", "Home/League/{id}/{teamId}", new { controller = "Home", action = "League", id = "", teamId =  UrlParameter.Optional });
            routes.MapReportingRoute();
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}