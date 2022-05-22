using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using User.Application.Handlers.Commands;

namespace User.Application
{
    public static class DependancyInjection
    {
        public static void RegisterUserApplication(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetAssembly(typeof(BaseCommand<>)), Assembly.GetAssembly(typeof(BaseCommandHandler<,>)));
        }
    }
}
