using Authentication.Application.Handlers.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Authentication.Application
{
    public static class DependancyInjection
    {
        public static void RegisterAuthenticationApplication(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetAssembly(typeof(BaseCommand<>)), Assembly.GetAssembly(typeof(BaseCommandHandler<,>)));
        }

    }
}
