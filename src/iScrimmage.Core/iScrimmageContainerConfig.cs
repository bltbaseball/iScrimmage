using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using iScrimmage.Core.Common;
using iScrimmage.Core.Data;
using iScrimmage.Core.Data.Mapping;

namespace iScrimmage.Core
{
    public class iScrimmageContainerConfig
    {
        public static ContainerBuilder RegisterContainer()
        {
            var builder = new ContainerBuilder();
            var asm = typeof(iScrimmageContainerConfig).Assembly;

            builder.RegisterType<SqlDataContext>().As<IDataContext>()
                .InstancePerDependency()
                .AsImplementedInterfaces()
                .AsSelf();

            builder.RegisterType<UserSessionProvider>()
                 .SingleInstance()
                 .As<IUserSessionProvider>()
                 .AsImplementedInterfaces()
                 .AsSelf();

            builder.RegisterType<ConnectionFactory>()
                .As<IConnectionFactory>()
                .SingleInstance()
                .AsImplementedInterfaces()
                .AsSelf();

            builder.RegisterAssemblyTypes(asm)
                   .AsClosedTypesOf(typeof(IMappingOverride<>));

            return builder;
        }
    }
}
