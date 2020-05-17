using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Take.Chat.Core.Entities;

namespace Take.Chat.Infrastructure
{
    public static class ContainerSetup
    {
		public static IServiceProvider InitializeWeb(Assembly webAssembly, IServiceCollection services) =>
			new AutofacServiceProvider(BaseAutofacInitialization(setupAction =>
			{
				setupAction.Populate(services);
				setupAction.RegisterAssemblyTypes(webAssembly).AsImplementedInterfaces();
			}));

		public static IContainer BaseAutofacInitialization(Action<ContainerBuilder> setupAction = null)
		{
			var builder = new ContainerBuilder();

			var coreAssembly = Assembly.GetAssembly(typeof(EntidadeBase));
			var infrastructureAssembly = Assembly.GetAssembly(typeof(AppSettingsOptions));
			builder.RegisterAssemblyTypes(coreAssembly, infrastructureAssembly).AsImplementedInterfaces();

			setupAction?.Invoke(builder);
			return builder.Build();
		}
	}
}
