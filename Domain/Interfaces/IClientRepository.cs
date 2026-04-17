using Domain.Entities;
using Domain.Models;
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
        Task<IEnumerable<Client>> GetFilteredClientsAsync(string filter);
        Task<PagedResult<Client>> GetPagedFilteredClientsAsync(int page, int pageSize, string filter);
        Task<Client?> GetClientByPhoneAsync(string phone);
        Task<Client?> GetClientByIdAsync(int id);
        Task<Client?> GetClientByUserIdAsync(string userId);
    }
}
