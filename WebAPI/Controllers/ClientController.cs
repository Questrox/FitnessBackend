using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController(ClientService _clientService, ILogger<ClientController> _logger) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<ClientDTO>>> GetClients()
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает список всех клиентов");

            var clients = await _clientService.GetClientsAsync();
            return Ok(clients);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<ClientDTO>>> GetSortedClientsByPhone(string phone)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает список клиентов с номером {phone}");

            var clients = await _clientService.GetSortedClientsByPhoneAsync(phone);
            return Ok(clients);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<ClientDTO?>> GetClientByPhoneAsync(string phone)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает клиента с номером {phone}");

            var client = await _clientService.GetClientByPhoneAsync(phone);
            if (client == null) return NotFound();
            return Ok(client);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<ClientDTO>> AddClient(CreateClientDTO client)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} создает клиента");

            var newClient = await _clientService.AddClientAsync(client);
            return Ok(newClient);
        }
        [HttpPut("[action]/{id}")]
        public async Task<ActionResult<ClientDTO>> UpdateClient(int id, ClientDTO client)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != client.Id) return BadRequest();

            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет клиента с id {id}");

            var updatedClient = await _clientService.UpdateClientAsync(client);
            return Ok(updatedClient);
        }
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет клиента с id {id}");

            await _clientService.DeleteClient(id);
            return NoContent();
        }
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> SoftDeleteClient(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} мягко удаляет клиента с id {id}");

            await _clientService.SoftDeleteClient(id);
            return NoContent();
        }
    }
}
