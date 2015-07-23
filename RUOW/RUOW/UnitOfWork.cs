using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly Dictionary<Type, object> repos;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UnitOfWork(DbContext context)
        {
            _context = context;
            repos = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>Instance of repository.</returns>
        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            IRepository<TEntity> repo = null;
            if (!repos.ContainsKey(typeof(TEntity)))
            {
                repo = new Repository<TEntity>(_context);
                repos.Add(typeof(TEntity), repos);
            }
            else
            {
                repo = (Repository<TEntity>)repos[typeof(TEntity)];
            }

            return repo;
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

        #region Private Methods

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
