using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RUOW
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        IList<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null,
                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sort = null,
                              params Expression<Func<TEntity, object>>[] include);

        TEntity GetByID(object id);

        TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter,
                                    params Expression<Func<TEntity, object>>[] include);

        void Create(TEntity entity, params TEntity[] entities);

        void Update(TEntity entity, params TEntity[] entities);

        void Delete(TEntity entity, params TEntity[] entities);

        void Delete(object id);

        void Delete(Expression<Func<TEntity, bool>> filter);

        int SaveChanges();
    }
}
