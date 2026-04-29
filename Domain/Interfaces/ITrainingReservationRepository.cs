using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITrainingReservationRepository : IRepository<TrainingReservation>
    {
        Task<IEnumerable<TrainingReservation>> GetClientReservationsAsync(int clientId);
        Task<TrainingReservation?> GetReservationByIdAsync(int id);
    }
}
