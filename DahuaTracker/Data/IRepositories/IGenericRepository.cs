using System.Linq.Expressions;

namespace DahuaTracker.Data.IRepositories
{
    public interface IGenericRepository<TSource> where TSource : class
    {
        IQueryable<TSource> GetAll(Expression<Func<TSource, bool>> expression = null);
        Task<TSource> AddAsync(TSource entity);
        Task<TSource> GetAsync(Expression<Func<TSource, bool>> expression);
        Task SaveChangesAsync();
        void Update(Expression<Func<TSource, bool>> expression, TSource entity);
        TSource Get(Expression<Func<TSource, bool>> expression);
    }
}
