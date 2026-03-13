using Domain.Entities.Old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IFoodRepository
    {
        Task<IEnumerable<Food>> GetAllFoodsForUserAsync(string userId);
        Task AddFoodAsync(Food food);
    }
}
