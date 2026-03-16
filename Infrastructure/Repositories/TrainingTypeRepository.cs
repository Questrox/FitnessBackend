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
    public class TrainingTypeRepository : Repository<TrainingType>, ITrainingTypeRepository
    {
        public TrainingTypeRepository(FitnessDb db) : base(db) { }
        public async Task<IEnumerable<TrainingType>> GetTrainingTypesAsync()
            => await _dbSet.ToListAsync();
        public async Task<TrainingType?> GetTrainingTypeByIdAsync(int id)
            => await _dbSet.FirstOrDefaultAsync(t => t.Id == id);
    }
}
