using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface INotificationRepository : IRepository<CancellationNotification>
    {
        Task<IEnumerable<CancellationNotification>> GetNotificationsAsync();
        Task<IEnumerable<CancellationNotification>> GetActiveNotificationsAsync();
        Task<CancellationNotification?> GetNotificationByIdAsync(int id);
    }
}
