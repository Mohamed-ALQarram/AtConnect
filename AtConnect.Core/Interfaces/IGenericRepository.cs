using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByKeysAsync(params object[] keys);
        IQueryable<T> GetAll();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }


}
