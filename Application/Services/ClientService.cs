using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ClientService(IClientRepository _clientRep)
    {
        public async Task<IEnumerable<ClientDTO>> GetClientsAsync()
        {
            var clients = await _clientRep.GetClientsAsync();
            return clients.Select(c => new ClientDTO(c));
        }
        public async Task<IEnumerable<ClientDTO>> GetFilteredClientsAsync(string filter)
        {
            var clients = await _clientRep.GetFilteredClientsAsync(filter);
            return clients.Select(c => new ClientDTO(c));
        }
        public async Task<ClientDTO?> GetClientByPhoneAsync(string phone)
        {
            var client = await _clientRep.GetClientByPhoneAsync(phone);
            return client == null ? null : new ClientDTO(client);
        }
        public async Task<ClientDTO?> GetClientByIdAsync(int id)
        {
            var client = await _clientRep.GetClientByIdAsync(id);
            return client == null ? null : new ClientDTO(client);
        }
        public async Task<ClientDTO?> GetClientByUserIdAsync(string userId)
        {
            var client = await _clientRep.GetClientByUserIdAsync(userId);
            return client == null ? null : new ClientDTO(client);
        }
        public async Task<ClientDTO> AddClientAsync(CreateClientDTO client)
        {
            var newClient = new Client
            {
                Bonuses = client.Bonuses,
                UserId = client.UserId
            };
            await _clientRep.AddAsync(newClient);
            newClient = await _clientRep.GetClientByIdAsync(newClient.Id);
            return new ClientDTO(newClient);
        }
        public async Task<ClientDTO> UpdateClientAsync(ClientDTO client)
        {
            var existingClient = await _clientRep.GetClientByIdAsync(client.Id) ??
                throw new KeyNotFoundException($"Клиент с Id {client.Id} не найден");
            existingClient.Bonuses = client.Bonuses;
            existingClient.UserId = client.UserId;

            await _clientRep.UpdateAsync(existingClient);
            return new ClientDTO(existingClient);
        }
        public async Task DeleteClient(int clientId)
        {
            var client = await _clientRep.GetClientByIdAsync(clientId) ?? 
                throw new KeyNotFoundException($"Клиент с Id {clientId} не найден");
            await _clientRep.DeleteAsync(client);
        }
        public async Task SoftDeleteClient(int clientId)
        {
            var client = await _clientRep.GetClientByIdAsync(clientId) ??
                throw new KeyNotFoundException($"Клиент с Id {clientId} не найден");
            await _clientRep.SoftDeleteAsync(client);
        }
    }
}
