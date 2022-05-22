using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserAuthentication.Domain.Authentications;

namespace Authentication.Infrastructure.Repository
{
    internal class AuthenticationDbContext : DbContext//IdentityDbContext<IdentityUser, IdentityRole, string>//DbContext
    {
        public DbSet<AuthenticationModel> Authentications { get; set; }

        public AuthenticationDbContext()
        {
        }

        public AuthenticationDbContext(DbContextOptions contextOptions) : base(contextOptions)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=AuthenticationDb;UID=sa;PWD=1369122#hamidEbr;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthenticationModel>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
