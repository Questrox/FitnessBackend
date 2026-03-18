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
    public class NotificationRepository : Repository<CancellationNotification>, INotificationRepository
    {
        public NotificationRepository(FitnessDb db) : base(db) { }
        public async Task<IEnumerable<CancellationNotification>> GetNotificationsAsync()
            => await _dbSet.Include(n => n.Client).Include(n => n.Training).ToListAsync();
        public async Task<IEnumerable<CancellationNotification>> GetActiveNotificationsAsync()
            => await _dbSet.Include(n => n.Client).Include(n => n.Training)
            .Where(n => !n.IsNotified).ToListAsync();
        public async Task<CancellationNotification?> GetNotificationByIdAsync(int id)
            => await _dbSet.Include(n => n.Client).Include(n => n.Training).FirstOrDefaultAsync(n => n.Id == id);
    }
}
