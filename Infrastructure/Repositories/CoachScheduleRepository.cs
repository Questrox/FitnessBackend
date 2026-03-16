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
    public class CoachScheduleRepository : Repository<CoachSchedule>, ICoachScheduleRepository
    {
        public CoachScheduleRepository(FitnessDb db) : base(db) { }
        public async Task<IEnumerable<CoachSchedule>> GetScheduleForCoachAsync(int coachId)
        {
            return await _dbSet.Where(c => c.CoachId == coachId).ToListAsync();
        }
        public async Task<CoachSchedule?> GetCoachScheduleByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
