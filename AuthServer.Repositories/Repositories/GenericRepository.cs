using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace AuthServer.Repositories.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class

    {
        protected readonly AuthServerDbContext _dbContext;

        private readonly DbSet<T> _dbSet;


        public GenericRepository(AuthServerDbContext dbContext)
        {
            _dbContext=dbContext;
            _dbSet=dbContext.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _dbContext.Remove(entity);
        }
        public IQueryable<T> GetAllAsync()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }
        public async Task<T> GetByIdAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity!=null)
            {
                _dbContext.Entry(entity).State=EntityState.Detached;

            }
            return entity;
        }
        public T Update(T entity)
        {
            _dbContext.Entry(entity).State=EntityState.Modified;
            return entity;
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }
    }
}
