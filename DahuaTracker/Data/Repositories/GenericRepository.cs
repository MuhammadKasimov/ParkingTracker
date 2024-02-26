using DahuaTracker.Contexts;
using DahuaTracker.Data.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DahuaTracker.Data.Repositories
{
    public class GenericRepository<TSource> : IGenericRepository<TSource> where TSource : class
    {
        protected readonly AppDbContext dbContext;
        protected readonly DbSet<TSource> dbSet;

        public GenericRepository()
        {
            this.dbContext = new AppDbContext();
            this.dbSet = dbContext.Set<TSource>();
        }

        public async Task<TSource> AddAsync(TSource entity)
            => (await dbSet.AddAsync(entity)).Entity;

        public IQueryable<TSource> GetAll(Expression<Func<TSource, bool>> expression = null)
            => expression is null ? dbSet.AsNoTracking() : dbSet.Where(expression).AsNoTracking();

        public async Task<TSource> GetAsync(Expression<Func<TSource, bool>> expression)
            => await GetAll(expression).FirstOrDefaultAsync();

        public async Task SaveChangesAsync()
            => await dbContext.SaveChangesAsync();

        public async void Update(Expression<Func<TSource, bool>> expression, TSource entity)
        {
            var existEntity = dbSet.FirstOrDefault(expression);
            if (existEntity != null)
                dbSet.Update(existEntity);
            else
                dbSet.Add(entity);
        }
        public TSource Get(Expression<Func<TSource, bool>> expression) =>
            dbSet.FirstOrDefault(expression);
        public void Clear()
        {
            foreach (var entity in dbSet)
                dbSet.Remove(entity);
        }
    }
}
