using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RUOW
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        /// <summary>
        /// Gets list of all entities.
        /// </summary>
        /// <param name="filter">Filter expression for filtering the entities.</param>
        /// <param name="sort">Sorting the entities.</param>
        /// <param name="include">Include for eager-loading.</param>
        /// <returns></returns>
        public virtual IList<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null,
                                      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sort = null,
                                       params Expression<Func<TEntity, object>>[] include)
        {
            IQueryable<TEntity> dbQuery = SelectQuery(filter, include);

            if (sort != null)
            {
                dbQuery = sort(dbQuery);
            }

            return dbQuery.AsNoTracking().ToList();
        }

        /// <summary>
        /// Gets the entity by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        /// <summary>
        /// Gets the first entity found or default value.
        /// </summary>
        /// <param name="filter">Filter expression for filtering the entities.</param>
        /// <param name="include">Include for eager-loading.</param>
        /// <returns></returns>
        public virtual TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter,
                                          params Expression<Func<TEntity, object>>[] include)
        {
            IQueryable<TEntity> dbQuery = SelectQuery(filter, include);
            return dbQuery.AsNoTracking().FirstOrDefault();
        }

        /// <summary>
        /// Creates the specified entity/entities.
        /// </summary>
        /// <param name="entity">Single entity.</param>
        /// <param name="entities">Multiple entities.</param>
        public virtual void Create(TEntity entity, params TEntity[] entities)
        {
            EntityState state = EntityState.Added;
            SetEntityState(state, entity, entities);
        }

        /// <summary>
        /// Updates the specified entity/entities.
        /// </summary>
        /// <param name="entity">Single entity.</param>
        /// <param name="entities">Multiple entities.</param>
        public virtual void Update(TEntity entity, params TEntity[] entities)
        {
            EntityState state = EntityState.Modified;
            SetEntityState(state, entity, entities);
        }

        /// <summary>
        /// Deletes the specified entity/entities.
        /// </summary>
        /// <param name="entity">Single entity.</param>
        /// <param name="entities">Multiple entities.</param>
        public virtual void Delete(TEntity entity, params TEntity[] entities)
        {
            EntityState state = EntityState.Deleted;
            SetEntityState(state, entity, entities);
        }

        /// <summary>
        /// Deletes the entity by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public virtual void Delete(object id)
        {
            TEntity entity = _dbSet.Find(id);
            EntityState state = EntityState.Deleted;
            SetEntityState(state, entity);
        }

        /// <summary>
        /// Deletes multiple entities which are found using filter.
        /// </summary>
        /// <param name="filter">Filter expression for filtering the entities.</param>
        public virtual void Delete(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> dbQuery = SelectQuery(filter);
            dbQuery.AsNoTracking().ToList().ForEach(item => _context.Entry(item).State = EntityState.Deleted);
        }

        /// <summary>
        /// Saves the changes to the database.
        /// </summary>
        /// <returns>Number of rows affected.</returns>
        public int SaveChanges()
        {
            int recordsAffected = _context.SaveChanges();
            this.Dispose();
            return recordsAffected;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Repository{TEntity}"/> class.
        /// </summary>
        ~Repository()
        {
            Dispose(false);
        }

        #region Private Methods

        private IQueryable<TEntity> SelectQuery(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>>[] include = null)
        {
            IQueryable<TEntity> dbQuery = _dbSet;

            if (filter != null)
            {
                dbQuery = dbQuery.Where(filter);
            }

            if (include != null)
            {
                dbQuery = include.Aggregate(dbQuery, (a, b) => a.Include(b));
            }
            return dbQuery;
        }

        private void SetEntityState(EntityState state, TEntity entity, params TEntity[] entities)
        {
            _context.Entry(entity).State = state;
            foreach (TEntity item in entities)
            {
                _context.Entry(item).State = state;
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                }
            }
        }

        #endregion Private Methods
    }
}
