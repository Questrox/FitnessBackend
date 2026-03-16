using Domain.Entities.Old;
using Domain.Interfaces.Old;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Old
{
    public class FoodEntryRepository : IFoodEntryRepository
    {
        private readonly CaloriesDb _db;
        public FoodEntryRepository(CaloriesDb db)
        {
            _db = db;
        }
        public async Task<IEnumerable<FoodEntry>> GetUserFoodEntriesByDateAsync(string userId, DateTime date)
        {
            return await _db.FoodEntries.Include(f => f.Food).Include(f => f.MealType)
                .Where(f => f.UserId == userId && f.Date == date).ToListAsync();
        }
        public async Task<FoodEntry> GetFoodEntryByIdAsync(int id)
        {
            return await _db.FoodEntries.Include(f => f.Food).Include(f => f.MealType).FirstOrDefaultAsync(f => f.Id == id);
        }
        public async Task AddFoodEntryAsync(FoodEntry entry)
        {
            _db.FoodEntries.Add(entry);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteFoodEntryAsync(int id)
        {
            var entry = await _db.FoodEntries.FirstOrDefaultAsync(f => f.Id == id);
            if (entry != null)
            {
                _db.FoodEntries.Remove(entry);
                await _db.SaveChangesAsync();
            }
        }
    }
}
