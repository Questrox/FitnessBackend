using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly FitnessDb _db;
        protected readonly DbSet<T> _dbSet;

        public Repository(FitnessDb db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(T entity)
        {
            if (entity is not ISoftDeletable deletable)
                throw new InvalidOperationException(
                    $"Сущность {typeof(T).Name} не поддерживает мягкое удаление");

            deletable.IsDeleted = true;

            _db.Entry(entity).State = EntityState.Modified;

            await _db.SaveChangesAsync();
        }
    }
}
