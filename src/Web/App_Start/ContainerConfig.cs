using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Http;
using System.Linq;
using System.Reflection;
using System.Web;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System.Web.Mvc;
using iScrimmage.Core;
using iScrimmage.Core.Common;
using Microsoft.Practices.ServiceLocation;
using Web.Filters;

namespace Web
{
    public class ContainerConfig
    {
        public static IContainer RegisterContainer<T>(T app) where T : System.Web.HttpApplication
        {
            var builder = iScrimmageContainerConfig.RegisterContainer();
            var asm = BuildManager.GetReferencedAssemblies().Cast<Assembly>();

            builder.RegisterType<WebApiAuthorizeAttribute>()
                .As<WebApiAuthorizeAttribute>()
                //.InstancePerApiRequest()
                .PropertiesAutowired(PropertyWiringOptions.PreserveSetValues);

            builder.RegisterApiControllers(asm.ToArray()).PropertiesAutowired();
            builder.RegisterFilterProvider();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            var resolver = new AutofacWebApiDependencyResolver(container);

            GlobalConfiguration.Configuration.DependencyResolver = resolver;

            var csl = new AutofacServiceLocator(container);

            ServiceLocator.SetLocatorProvider(() => csl);
            Ioc.Current = container;

            return container;
        }
    }
}