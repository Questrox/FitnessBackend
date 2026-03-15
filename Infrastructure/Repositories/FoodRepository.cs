using Domain.Entities.Old;
using Domain.Interfaces.Old;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FoodRepository : IFoodRepository
    {
        private readonly CaloriesDb _db;
        public FoodRepository(CaloriesDb db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Food>> GetAllFoodsForUserAsync(string userId)
        {
            return await _db.Foods.Where(f => f.UserId == userId || f.UserId == null).ToListAsync();
        }
        public async Task AddFoodAsync(Food food)
        {
            _db.Foods.Add(food);
            await _db.SaveChangesAsync();
        }
    }
}
