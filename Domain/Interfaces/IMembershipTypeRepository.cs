using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMembershipTypeRepository : IRepository<MembershipType>
    {
        Task<IEnumerable<MembershipType>> GetMembershipTypesAsync();
        Task<MembershipType?> GetMembershipTypeById(int id);
    }
}
