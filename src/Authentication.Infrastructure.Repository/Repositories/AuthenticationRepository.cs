using UserAuthentication.Domain.Authentications;

namespace Authentication.Infrastructure.Repository.Repositories
{
    internal class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly AuthenticationDbContext _dbContext;

        public AuthenticationRepository(AuthenticationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AuthenticationModel> Add(AuthenticationModel entity)
        {
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public Task<List<AuthenticationModel>> FindAll()
        {
            return Task.FromResult(_dbContext.Set<AuthenticationModel>().ToList());
        }

        public async Task<AuthenticationModel> FindByIdAsync(long id) => await _dbContext.FindAsync<AuthenticationModel>(id);

        public Task Remove(long id)
        {
            _dbContext.Remove(new AuthenticationModel() { Id = id });
            return Task.CompletedTask;
        }
    }
}
