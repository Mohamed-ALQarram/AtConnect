using AtConnect.Core.Interfaces;
using AtConnect.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext appDbContext;

        public GenericRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task AddAsync(T entity)
        {
            await appDbContext.AddAsync<T>(entity);
        }

        public void Delete(T entity)
        {
           appDbContext.Set<T>().Remove(entity);
        }

        public IQueryable<T> GetAll()
        {
            return appDbContext.Set<T>();
        }

        public async Task<T?> GetByKeysAsync(params object[] keys)
        {
            return await appDbContext.Set<T>().FindAsync(keys);
        }

        public void Update(T entity)
        {
            appDbContext.Set<T>().Update(entity);
        }
    }
}
