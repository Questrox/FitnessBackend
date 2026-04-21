using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITrainingRepository : IRepository<Training>
    {
        Task<IEnumerable<Training>> GetTrainingsForPeriodAsync(DateTime start, DateTime end);
        Task<Training?> GetTrainingByIdAsync(int id);
        Task<IEnumerable<Training>> GetTrainingsWithNotificationsAsync();
    }
}
