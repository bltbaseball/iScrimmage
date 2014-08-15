using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using Autofac;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Context;
using System.Web.Security;
using Web.Models;

namespace Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static ISessionFactory SessionFactory { get; private set; }
        public static IContainer Container { get; protected set; }

        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();

            SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));
            Container = ContainerConfig.RegisterContainer(this);

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            var nhConfig = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008
                    .ConnectionString(connstr => connstr.FromConnectionStringWithKey("DefaultConnection"))
                    .AdoNetBatchSize(100)
                    //.Dialect<NHibernate.Spatial.Dialect.MsSql2008GeographyDialect>()
                )
                .ExposeConfiguration(BuildSchema)
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.DivisionMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.LeagueMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.PlayerMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.TeamClassMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.TeamMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.TeamPlayerMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.UserMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.OAuthMembershipMapping>())
                //.Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.UsersProfileMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.ManagerMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.CoachMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.UmpireMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.GameMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.LocationMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.GuardianMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.PayPalPaymentMapping>())
                //.Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.TournamentMapping>())
                //.Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.TournamentRegistrationMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.FeeMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.FeePaymentMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.AvailableDateMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.BracketResultMapping>())
                .Mappings(mappings=>mappings.HbmMappings.AddFromAssemblyOf<Web.Models.Mappings.BracketResultMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.BracketMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.BracketTeamMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.MessageLogMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.PlayerGameStatMapping>())
                .Mappings(mappings => mappings.FluentMappings.Add<Web.Models.Mappings.BracketGeneratorMapping>())

                //.Mappings(mappings => mappings.FluentMappings.AddFromAssemblyOf<UserMapping>())
                .BuildConfiguration();
            SessionFactory = nhConfig.BuildSessionFactory();
        }

        private static void BuildSchema(NHibernate.Cfg.Configuration config)
        {
            config.SetProperty("current_session_context_class", "web");
            //new NHibernate.Tool.hbm2ddl.SchemaExport(config).SetOutputFile(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/schema.sql")).Create(true, false);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var session = SessionFactory.OpenSession();
            CurrentSessionContext.Bind(session);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            var session = CurrentSessionContext.Unbind(SessionFactory);
            session.Dispose();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        // Get Forms Identity From Current User
                        FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
                        // Create a custom Principal Instance and assign to Current User (with caching)
                        BLTBPrincipal principal = (BLTBPrincipal)HttpContext.Current.Cache.Get("User-" + id.Name);
                        if (principal == null)
                        {
                            // Create and populate your Principal object with the needed data and Roles.
                            principal = BLTBPrincipal.CreatePrincipal(id, id.Name);

                            if (principal == null)
                            {
                                // this can occur if the user was deleted by an admin while still logged in
                                // so not a valid user, log the user out and abandon their session to make
                                // them re-login
                                if (HttpContext.Current.Session != null)
                                {
                                    HttpContext.Current.Session.Abandon();
                                }
                                FormsAuthentication.SignOut();
                                HttpContext.Current.User = null;
                                return;
                            }

                            HttpContext.Current.Cache.Add(
                                 "User-" + id.Name,
                                 principal,
                                 null,
                                 System.Web.Caching.Cache.NoAbsoluteExpiration,
                                 new TimeSpan(0, 30, 0),
                                 System.Web.Caching.CacheItemPriority.Default,
                                 null);
                        }

                        HttpContext.Current.User = principal;
                    }
                }
            }
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
        }
    }
}