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
    public class TrainingReservationRepository : Repository<TrainingReservation>, ITrainingReservationRepository
    {
        public TrainingReservationRepository(FitnessDb db) : base(db) { }
        public async Task<IEnumerable<TrainingReservation>> GetClientReservations(int clientId)
            => await _dbSet.Include(t => t.Training).Include(t => t.Client).Include(t => t.Payment)
            .Where(t => t.ClientId == clientId).ToListAsync();
        public async Task<TrainingReservation?> GetReservationById(int id)
            => await _dbSet.Include(t => t.Training).Include(t => t.Client).Include(t => t.Payment)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
