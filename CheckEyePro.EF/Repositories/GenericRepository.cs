using CheckEyePro.EF.DBContext;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Core.IRepository;
using System.Linq.Expressions;

namespace OrderManagementSystem.EF.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {           
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public T Delete(T entity)
        {
            
            _context.Set<T>().Remove(entity);
            return entity;
        }
        public async Task<T> FindAsync(Expression<Func<T, bool>> craiteria, string[] includes = null)
        {
            IQueryable<T> Query = _context.Set<T>();
            if (includes != null)
                foreach (var include in includes)
                    Query = Query.Include(include);
            
            return await Query.FirstOrDefaultAsync(craiteria);
        }

            public async Task<IEnumerable<T>> FindAllWithcraiteriaAsync(Expression<Func<T, bool>> craiteria, string[] includes = null)
        {
            IQueryable<T> Query = _context.Set<T>();
            if (includes != null)
                foreach (var include in includes)
                    Query = Query.Include(include);
            
            return await Query.Where(craiteria).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllWithIncludesAsync(string[] includes)
        {
            IQueryable<T> Query = _context.Set<T>();
            if (includes != null)
                foreach (var include in includes)
                    Query = Query.Include(include);

            return await Query.ToListAsync();
        }
        public  async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int Id)
        {
            return await _context.Set<T>().FindAsync(Id);
        }

        public T Update(T entity)
        {
            _context.Set<T>().Update(entity);  
            return entity;
        }
    }
}
