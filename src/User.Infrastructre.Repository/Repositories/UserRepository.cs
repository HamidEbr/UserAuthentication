/*
     * Repository: Mediates between the domain and data mapping layers 
     * using a collection-like interface for accessing domain objects. 
     * https://martinfowler.com/eaaCatalog/repository.html
     * 
     * This is the implementation for ITaskRepository which needs to
     * implement the generic IRepository actions. 
     * Both are located in Domain layer given that they are interfaces
     * attached to Task domain (these interfaces are called ports in
     * hexagonal architecture and the implementation in this class is
     * called adapter)
     * 
     * With this architecture pattern your data access code can be changed
     * easily only performing the changes in this class. 
     * You may want to use a MongoDB, SQL or whatever, you just need to 
     * change it here.
     */

using User.Infrastructre.Repository;
using UserAuthentication.Domain.Users;

namespace User.Infrastructure.Repository.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly UserDbContext _dbContext;

        public UserRepository(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserModel> Add(UserModel entity)
        {
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public Task<List<UserModel>> FindAll()
        {
            return Task.FromResult(_dbContext.Set<UserModel>().ToList());
        }

        public async Task<UserModel> FindByIdAsync(long id) => await _dbContext.FindAsync<UserModel>(id);

        public Task Remove(long id)
        {
            _dbContext.Remove(new UserModel() { Id = id });
            return Task.CompletedTask;
        }
    }
}
