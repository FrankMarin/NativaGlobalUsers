using Microsoft.EntityFrameworkCore;
using NativaGlobalUsers.Models;
using System.Diagnostics;
using System.Linq.Expressions;

namespace NativaGlobalUsers.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;
        internal DbSet<T> DbSet { get; set; }


        public Repository(ApplicationDbContext context)
        {
            this.context = context;
            this.DbSet = this.context.Set<T>();
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task Create(T entity)
        {
            await this.context.AddAsync(entity);
            await Save();
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter = null, bool tracked = true)
        {
            IQueryable<T> query = DbSet;
            if (tracked)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task Remove(T entidad)
        {
            this.DbSet.Remove(entidad);
            await this.Save();
        }

        public async Task<User> UpdateUser(User entity)
        {
            this.context.Update(entity);
            await this.context.SaveChangesAsync();

            return entity;
        }

        public async Task Save()
        {
            await this.context.SaveChangesAsync();
        }
    }
}
