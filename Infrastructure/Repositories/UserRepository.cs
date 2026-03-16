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
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(FitnessDb db) : base(db) { }
        public async Task<User?> GetUserByIdAsync(string id)
            => await _dbSet.FirstOrDefaultAsync(u => u.Id == id);
    }
}
