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
    public class MembershipTypeRepository : Repository<MembershipType>, IMembershipTypeRepository
    {
        public MembershipTypeRepository(FitnessDb db) : base(db) { }
        public async Task<IEnumerable<MembershipType>> GetMembershipTypesAsync() =>
            await _dbSet.ToListAsync();
        public async Task<MembershipType?> GetMembershipTypeById(int id) =>
            await _dbSet.FirstOrDefaultAsync(m => m.Id == id);
    }
}
