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
    public class CoachRepository : Repository<Coach>, ICoachRepository
    {
        public CoachRepository(FitnessDb db) : base(db) { }
        public async Task<IEnumerable<Coach>> GetCoachesAsync()
        {
            return await _dbSet.Include(c => c.CoachSchedules).Include(c => c.User).ToListAsync();
        }
        public async Task<Coach?> GetCoachByIdAsync(int id)
        {
            return await _dbSet.Include(c => c.CoachSchedules).Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
