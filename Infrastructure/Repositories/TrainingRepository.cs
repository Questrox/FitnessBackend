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

        // Возвращаем либо все групповые, либо все групповые + индивидуальные для конкретного тренера
        public async Task<IEnumerable<Training>> GetTrainingsForPeriodAsync(DateTime start, DateTime end, int? coachId)
            => await _dbSet.Include(t => t.TrainingType).Include(t => t.Coach).ThenInclude(c => c.User).Include(t => t.TrainingStatus)
            .Include(t => t.TrainingReservations).ThenInclude(tr => tr.Client).ThenInclude(c => c.User)
            .Where(t => t.StartDate.Date >= start && t.EndDate.Date <= end && t.TrainingStatusId != (int)TrainingStatusEnum.Cancelled &&
            (t.TrainingType.MaxClients > 1 || coachId != null && t.CoachId == coachId)
            ).ToListAsync();
        public async Task<Training?> GetTrainingByIdAsync(int id)
            => await _dbSet.Include(t => t.TrainingType).Include(t => t.Coach).ThenInclude(c => c.User).Include(t => t.TrainingStatus)
            .Include(t => t.TrainingReservations).ThenInclude(tr => tr.Client).ThenInclude(c => c.User)
            .FirstOrDefaultAsync(t => t.Id == id);
        public async Task<IEnumerable<Training>> GetTrainingsWithNotificationsAsync()
        {
            return await _dbSet
                .Include(t => t.TrainingType)
                .Include(t => t.Coach).ThenInclude(c => c.User)
                .Include(t => t.CancellationNotifications).ThenInclude(n => n.Client).ThenInclude(c => c.User)
                .Include(t => t.CancellationNotifications).ThenInclude(c => c.Admin)
                .Where(t => t.TrainingStatusId == (int)TrainingStatusEnum.Cancelled)
                .ToListAsync();
        }
    }
}
