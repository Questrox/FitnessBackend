using Domain.Entities.Old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IWaterEntryRepository
    {
        Task<IEnumerable<WaterEntry>> GetUserWaterEntriesByDateAsync(string userId, DateTime date);
        Task AddWaterEntryAsync(WaterEntry waterEntry);
        Task DeleteWaterEntryAsync(int id);
    }
}
