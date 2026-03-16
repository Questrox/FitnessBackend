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
    public class WaterEntryRepository : IWaterEntryRepository
    {
        private readonly CaloriesDb _db;
        public WaterEntryRepository(CaloriesDb db)
        {
            _db = db;
        }
        public async Task<IEnumerable<WaterEntry>> GetUserWaterEntriesByDateAsync(string userId, DateTime date)
        {
            return await _db.WaterEntries.Where(w => w.UserId == userId && w.Date == date).ToListAsync();
        }
        public async Task AddWaterEntryAsync(WaterEntry entry)
        {
            _db.WaterEntries.Add(entry);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteWaterEntryAsync(int id)
        {
            var entry = await _db.WaterEntries.FirstOrDefaultAsync(f => f.Id == id);
            if (entry != null)
            {
                _db.WaterEntries.Remove(entry);
                await _db.SaveChangesAsync();
            }
        }
    }
}
