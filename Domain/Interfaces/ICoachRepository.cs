using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICoachRepository : IRepository<Coach>
    {
        Task<IEnumerable<Coach>> GetCoachesAsync();
        Task<Coach?> GetCoachByIdAsync(int id);
    }
}
