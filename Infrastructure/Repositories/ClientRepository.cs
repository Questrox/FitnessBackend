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
                .ToListAsync();
        }
        public async Task<IEnumerable<Client>> GetSortedClientsByPhoneAsync(string phone)
        {
            return await _dbSet.Include(c => c.Memberships).ThenInclude(m => m.MembershipType)
                .Include(c => c.TrainingReservations).ThenInclude(c => c.Training)
                .Where(c => c.PhoneNumber.Contains(phone)).ToListAsync();
        }
        public async Task<Client?> GetClientByPhoneAsync(string phone)
        {
            return await _dbSet.Include(c => c.Memberships).ThenInclude(m => m.MembershipType)
                .Include(c => c.TrainingReservations).ThenInclude(c => c.Training)
                .FirstOrDefaultAsync(c => c.PhoneNumber == phone);
        }
        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _dbSet.Include(c => c.Memberships).ThenInclude(m => m.MembershipType)
                .Include(c => c.TrainingReservations).ThenInclude(c => c.Training)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
