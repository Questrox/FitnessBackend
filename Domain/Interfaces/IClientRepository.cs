using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<IEnumerable<Client>> GetClientsAsync();
        Task<IEnumerable<Client>> GetFilteredClientsAsync(string phone);
        Task<Client?> GetClientByPhoneAsync(string phone);
        Task<Client?> GetClientByIdAsync(int id);
        Task<Client?> GetClientByUserIdAsync(string userId);
    }
}
