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
    public class DictionariesRepository(FitnessDb _db) : IDictionariesRepository
    {
        public async Task<IEnumerable<ReservationStatus>> GetReservationStatusesAsync()
        {
            return await _db.ReservationStatuses.ToListAsync();
        }
        public async Task<IEnumerable<TrainingStatus>> GetTrainingStatusesAsync()
        {
            return await _db.TrainingStatuses.ToListAsync();
        }
    }
}
