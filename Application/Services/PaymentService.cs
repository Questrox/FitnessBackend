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
    public class PaymentService(IPaymentRepository _paymentRep, IClientRepository _clientRep)
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
            var client = await _clientRep.GetClientByIdAsync(dto.ClientId);
            if (client == null)
                throw new ArgumentException($"Не найден клиент с id {dto.ClientId}");
            if (dto.AdminId == null)
                throw new ArgumentException("Нет id администратора");
            if (dto.PaidWithBonuses < 0)
                throw new ArgumentException($"Количество бонусов не может быть отрицательным. " +
                    $"Текущее количество: {dto.PaidWithBonuses}");
            if (dto.PaidWithBonuses > dto.Price)
                throw new ArgumentException($"Количество бонусов не может превышать цену. " +
                    $"Текущее количество: {dto.PaidWithBonuses}, цена: {dto.Price}");
            if (dto.PaidWithBonuses > client.Bonuses)
                throw new ArgumentException($"Клиенту не хватает бонусов для оплаты. " +
                    $"Введено количество для оплаты: {dto.PaidWithBonuses}, у клиента имеется: {client.Bonuses}");

            var payment = new Payment
            {
                Date = DateTime.Now,
                Price = dto.Price,
                CashbackPercentage = dto.CashbackPercentage,
                PaidWithBonuses = dto.PaidWithBonuses,
                AdminId = dto.AdminId,
            };

            await _paymentRep.AddAsync(payment);
            client.Bonuses -= dto.PaidWithBonuses;
            client.Bonuses += dto.Price / 100 * dto.CashbackPercentage;
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
        public async Task SoftDeletePayment(int id)
        {
            var payment = await _paymentRep.GetPaymentById(id) ??
                throw new KeyNotFoundException($"Платеж с Id {id} не найден");

            await _paymentRep.SoftDeleteAsync(payment);
        }
    }
}
