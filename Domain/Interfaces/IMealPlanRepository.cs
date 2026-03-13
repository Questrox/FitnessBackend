using Domain.Entities.Old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMealPlanRepository
    {
        Task<IEnumerable<MealPlan>> GetMealPlansAsync();
        Task<MealPlan> GetMealPlanAsync(int id);
    }
}
