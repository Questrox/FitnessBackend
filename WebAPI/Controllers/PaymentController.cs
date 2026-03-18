using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(PaymentService _paymentService, ILogger<PaymentController> _logger) : ControllerBase
    {
        [HttpGet("[action]/{clientId}")]
        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetClientPayments(int clientId)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает платежи клиента с id {clientId}");

            var payments = await _paymentService.GetClientPaymentsAsync(clientId);
            return Ok(payments);
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<PaymentDTO?>> GetPaymentById(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает платеж с id {id}");

            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<PaymentDTO>> AddPayment(CreatePaymentDTO payment)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} создаёт платеж");

            var newPayment = await _paymentService.AddPaymentAsync(payment);
            return Ok(newPayment);
        }

        [HttpPut("[action]/{id}")]
        public async Task<ActionResult<PaymentDTO>> UpdatePayment(int id, PaymentDTO payment)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != payment.Id) return BadRequest();

            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет платеж с id {id}");

            var updatedPayment = await _paymentService.UpdatePaymentAsync(payment);
            return Ok(updatedPayment);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет платеж с id {id}");

            await _paymentService.DeletePayment(id);
            return NoContent();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> SoftDeletePayment(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} мягко удаляет платеж с id {id}");

            await _paymentService.SoftDeletePayment(id);
            return NoContent();
        }
    }
}
