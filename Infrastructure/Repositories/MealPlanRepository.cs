using Domain.Entities.Old;
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
    public class MealPlanRepository : IMealPlanRepository
    {
        private readonly CaloriesDb _db;
        public MealPlanRepository(CaloriesDb db)
        {
            _db = db;
        }
        public async Task<IEnumerable<MealPlan>> GetMealPlansAsync()
        {
            return await _db.MealPlans.Include(m => m.MealPlanDay).ToListAsync();
        }
        public async Task<MealPlan> GetMealPlanAsync(int id)
        {
            return await _db.MealPlans.Include(m => m.MealPlanDay).FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
