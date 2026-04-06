using Application.Models;
using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ClientService(FitnessDb _db, AuthService _authService, IClientRepository _clientRep)
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
        public async Task<AddClientResult> AddClientAsync(CreateClientDTO client)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var userName = await _authService.GenerateUsernameAsync();
                var password = _authService.GeneratePassword();
                RegisterModel model = new RegisterModel
                {
                    FullName = client.FullName,
                    PhoneNumber = client.PhoneNumber,
                    UserName = userName,
                    Password = password
                };
                var result = await _authService.RegisterAsync(model);
                if (result == null)
                    throw new ArgumentException("Не удалось создать пользователя");
                var newClient = new Client
                {
                    Bonuses = 0,
                    UserId = result
                };
                await _clientRep.AddAsync(newClient);

                await transaction.CommitAsync();

                //newClient = await _clientRep.GetClientByIdAsync(newClient.Id);

                var addResult = new AddClientResult
                {
                    ClientId = newClient.Id,
                    UserName = userName,
                    Password = password
                };
                return addResult;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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
