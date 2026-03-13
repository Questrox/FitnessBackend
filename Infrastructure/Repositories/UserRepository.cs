using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CaloriesDb _db;
        public UserRepository(CaloriesDb db)
        {
            _db = db;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _db.Users.Include(u => u.MealPlan).ThenInclude(m => m.MealPlanDay).FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<User> UpdateUserAsync(User user)
        {
            _db.Entry(user).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return await _db.Users.FindAsync(user.Id); // Загружаем обновленный объект из БД
        }
    }
}