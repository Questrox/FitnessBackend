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
    public class TrainingRepository : Repository<Training>, ITrainingRepository
    {
        public TrainingRepository(FitnessDb db) : base(db) { }
        public async Task<IEnumerable<Training>> GetTrainingsForPeriodAsync(DateTime start, DateTime end)
            => await _dbSet.Include(t => t.TrainingType).Include(t => t.Coach)
            .Where(t => t.Date >= start && t.Date <= end && t.TrainingStatusId != 3).ToListAsync();
        public async Task<Training?> GetTrainingByIdAsync(int id)
            => await _dbSet.Include(t => t.TrainingType).Include(t => t.Coach).FirstOrDefaultAsync(t => t.Id == id);
    }
}
