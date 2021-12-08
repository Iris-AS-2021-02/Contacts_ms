using System.Linq.Expressions;

namespace DataAccess.RepositoryContracts
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void DeleteById(object id);

        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = null);

        TEntity GetById(object id);

        TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter = null, string includeProperties = null);

        TEntity Insert(TEntity entity);
    }
}
