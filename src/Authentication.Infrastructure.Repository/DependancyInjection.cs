using Authentication.Infrastructure.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserAuthentication.Domain.Authentications;

namespace Authentication.Infrastructure.Repository
{
    public static class DependancyInjection
    {
        public static void RegisterAuthenticationRepository(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthenticationDbContext>();
                //(options =>
                //options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
        }
    }
}
