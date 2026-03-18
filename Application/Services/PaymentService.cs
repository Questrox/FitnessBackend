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
    public class PaymentService(IPaymentRepository _paymentRep)
    {
        public async Task<IEnumerable<PaymentDTO>> GetClientPaymentsAsync(int clientId)
        {
            var payments = await _paymentRep.GetClientPayments(clientId);
            return payments.Select(p => new PaymentDTO(p));
        }

        public async Task<PaymentDTO?> GetPaymentByIdAsync(int id)
        {
            var payment = await _paymentRep.GetPaymentById(id);
            return payment == null ? null : new PaymentDTO(payment);
        }

        public async Task<PaymentDTO> AddPaymentAsync(CreatePaymentDTO dto)
        {
            var payment = new Payment
            {
                Date = dto.Date,
                Price = dto.Price,
                CashbackPercentage = dto.CashbackPercentage,
                PaidWithBonuses = dto.PaidWithBonuses
            };

            await _paymentRep.AddAsync(payment);
            payment = await _paymentRep.GetPaymentById(payment.Id);

            return new PaymentDTO(payment);
        }

        public async Task<PaymentDTO> UpdatePaymentAsync(PaymentDTO dto)
        {
            var existing = await _paymentRep.GetPaymentById(dto.Id) ??
                throw new KeyNotFoundException($"Платеж с Id {dto.Id} не найден");

            existing.Date = dto.Date;
            existing.Price = dto.Price;
            existing.CashbackPercentage = dto.CashbackPercentage;
            existing.PaidWithBonuses = dto.PaidWithBonuses;

            await _paymentRep.UpdateAsync(existing);

            return new PaymentDTO(existing);
        }

        public async Task DeletePayment(int id)
        {
            var payment = await _paymentRep.GetPaymentById(id) ??
                throw new KeyNotFoundException($"Платеж с Id {id} не найден");

            await _paymentRep.DeleteAsync(payment);
        }
    }
}
