using System;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Startup
    {
        public static IServiceProvider ConvertToAutofac(
            this IServiceCollection services,
            params IModule[] modules
        )
        {
            var container = new ContainerBuilder();
            foreach (var module in modules)
            {
                container.RegisterModule(module);
            }
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }
    }
}
