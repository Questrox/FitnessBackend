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
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(FitnessDb db) : base(db) { }
        public async Task<IEnumerable<Client>> GetClientsAsync()
        {
            return await _dbSet.Include(c => c.Memberships).ThenInclude(m => m.MembershipType)
                .Include(c => c.TrainingReservations).ThenInclude(c => c.Training)
                .Include(c => c.User)
                .ToListAsync();
        }
        public async Task<IEnumerable<Client>> GetFilteredClientsAsync(string filter)
        {
            return await _dbSet.Include(c => c.Memberships).ThenInclude(m => m.MembershipType)
                .Include(c => c.TrainingReservations).ThenInclude(c => c.Training)
                .Include(c => c.User)
                .Where(c => c.User.PhoneNumber.Contains(filter) || c.User.FullName.Contains(filter) || c.User.UserName.Contains(filter))
                .ToListAsync();
        }
        public async Task<Client?> GetClientByPhoneAsync(string phone)
        {
            return await _dbSet.Include(c => c.Memberships).ThenInclude(m => m.MembershipType)
                .Include(c => c.TrainingReservations).ThenInclude(c => c.Training)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User.PhoneNumber == phone);
        }
        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _dbSet.Include(c => c.Memberships).ThenInclude(m => m.MembershipType)
                .Include(c => c.Memberships).ThenInclude(c => c.Payment)
                .Include(c => c.TrainingReservations).ThenInclude(tr => tr.Training).ThenInclude(t => t.TrainingType)
                .Include(c => c.TrainingReservations).ThenInclude(tr => tr.ReservationStatus)
                .Include(c => c.TrainingReservations).ThenInclude(tr => tr.Payment)
                .Include(c => c.TrainingReservations).ThenInclude(tr => tr.Training).ThenInclude(t => t.Coach).ThenInclude(c => c.User)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Client?> GetClientByUserIdAsync(string userId)
        {
            return await _dbSet.Include(c => c.Memberships).ThenInclude(m => m.MembershipType)
                .Include(c => c.Memberships).ThenInclude(c => c.Payment)
                .Include(c => c.TrainingReservations).ThenInclude(tr => tr.Training).ThenInclude(t => t.TrainingType)
                .Include(c => c.TrainingReservations).ThenInclude(tr => tr.ReservationStatus)
                .Include(c => c.TrainingReservations).ThenInclude(tr => tr.Payment)
                .Include(c => c.TrainingReservations).ThenInclude(tr => tr.Training).ThenInclude(t => t.Coach).ThenInclude(c => c.User)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
