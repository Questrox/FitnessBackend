using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICoachScheduleRepository : IRepository<CoachSchedule>
    {
        Task<IEnumerable<CoachSchedule>> GetScheduleForCoachAsync(int coachId);
        Task<CoachSchedule?> GetCoachScheduleByIdAsync(int id);
    }
}
