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
    public class MembershipRepository : Repository<Membership>, IMembershipRepository
    {
        public MembershipRepository(FitnessDb db) : base(db) { }
        public async Task<IEnumerable<Membership>> GetMembershipsAsync()
        {
            return await _dbSet.Include(m => m.MembershipType).Include(m => m.Payment).ToListAsync();
        }
        public async Task<IEnumerable<Membership>> GetClientMembershipsAsync(int clientId)
        {
            return await _dbSet.Include(m => m.MembershipType).Include(m => m.Payment)
                .Where(m => m.ClientId == clientId).ToListAsync();
        }
        public async Task<Membership?> GetMembershipByIdAsync(int id)
        {
            return await _dbSet.Include(m => m.MembershipType).Include(m => m.Payment).FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
