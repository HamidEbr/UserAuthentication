using System.Collections.Generic;
using System.Threading.Tasks;

/*
 * Repository: Mediates between the domain and data mapping layers 
 * using a collection-like interface for accessing domain objects. 
 * https://martinfowler.com/eaaCatalog/repository.html
 * 
 * This is a generic interface for repositories to be used
 */

namespace UserAuthentication.Domain
{
    public interface IRepository<TEntity>
        where TEntity : IAggregateRoot
    {
        Task<TEntity> FindByIdAsync(long id);
        Task<List<TEntity>> FindAll();
        Task<TEntity> Add(TEntity entity);
        Task Remove(long id);
    }
}
