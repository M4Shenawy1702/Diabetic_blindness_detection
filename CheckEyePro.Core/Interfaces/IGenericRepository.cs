using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Core.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        public Task<T> GetByIdAsync(int Id);
        public Task<T> AddAsync(T entity);
        public T Delete(T entity);
        public T Update(T entity);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> FindAsync(Expression<Func<T, bool>> craiteria, string[] includes = null);
        public Task<IEnumerable<T>> FindAllWithcraiteriaAsync(Expression<Func<T, bool>> craiteria, string[] includes = null); 
        public Task<IEnumerable<T>> FindAllWithIncludesAsync(string[] includes);
    }
}
