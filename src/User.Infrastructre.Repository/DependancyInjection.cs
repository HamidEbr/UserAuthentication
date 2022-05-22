using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using User.Infrastructure.Repository.Repositories;
using UserAuthentication.Domain.Users;

namespace User.Infrastructre.Repository
{
    public static class DependancyInjection
    {
        public static void RegisterUserRepository(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
