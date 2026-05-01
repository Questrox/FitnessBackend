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
            return await _dbSet.Include(c => c.CoachSchedules).Include(c => c.User).Include(c => c.Trainings).ToListAsync();
        }
        public async Task<Coach?> GetCoachByUserIdAsync(string userId)
        {
            return await _dbSet.Include(c => c.CoachSchedules).Include(c => c.User).Include(c => c.Trainings).FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async Task<IEnumerable<Coach>> GetAvailableCoachesAsync(DateTime start, DateTime end)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.CoachSchedules)
                .Include(c => c.Trainings)
                .Where(c =>
                    c.CoachSchedules.Any(cs =>
                        cs.WeekDay == start.DayOfWeek &&
                        cs.StartTime <= start.TimeOfDay &&
                        cs.EndTime >= end.TimeOfDay
                    )
                    &&
                    !c.Trainings.Any(t =>
                        t.TrainingStatusId != (int)TrainingStatusEnum.Cancelled &&
                        t.StartDate < end &&
                        t.EndDate > start
                    )
                )
                .ToListAsync();
        }
        public async Task<Coach?> GetCoachByIdAsync(int id)
        {
            return await _dbSet.Include(c => c.CoachSchedules).Include(c => c.User).Include(c => c.Trainings).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
