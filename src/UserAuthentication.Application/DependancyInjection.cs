using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UserAuthentication.Application.Handlers.Commands;

namespace UserAuthentication.Application
{
    public static class DependancyInjection
    {
        public static void RegisterUserAuthenticationApplication(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetAssembly(typeof(BaseCommand<>)), Assembly.GetAssembly(typeof(BaseCommandHandler<,>)));
        }
    }
}
