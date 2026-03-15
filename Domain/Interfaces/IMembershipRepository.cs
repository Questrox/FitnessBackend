using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMembershipRepository : IRepository<Membership>
    {
        Task<IEnumerable<Membership>> GetMembershipsAsync();
        Task<IEnumerable<Membership>> GetClientMembershipsAsync(int clientId);
        Task<Membership?> GetMembershipByIdAsync(int id);
    }
}
