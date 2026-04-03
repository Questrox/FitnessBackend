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
    public class MembershipService(FitnessDb _db, PaymentService _paymentService, 
        IMembershipRepository _membershipRep, IMembershipTypeRepository _membershipTypeRep)
    {
        public async Task<IEnumerable<MembershipDTO>> GetMembershipsAsync()
        {
            var memberships = await _membershipRep.GetMembershipsAsync();
            return memberships.Select(m => new MembershipDTO(m));
        }

        public async Task<IEnumerable<MembershipDTO>> GetClientMembershipsAsync(int clientId)
        {
            var memberships = await _membershipRep.GetClientMembershipsAsync(clientId);
            return memberships.Select(m => new MembershipDTO(m));
        }

        public async Task<MembershipDTO?> GetMembershipByIdAsync(int id)
        {
            var membership = await _membershipRep.GetMembershipByIdAsync(id);
            return membership == null ? null : new MembershipDTO(membership);
        }

        public async Task<MembershipDTO> AddMembershipAsync(CreateMembershipDTO membership)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var mt = await _membershipTypeRep.GetMembershipTypeById(membership.MembershipTypeId);
                if (mt == null)
                    throw new ArgumentException($"Не найден тип абонемента с id {membership.MembershipTypeId}");
                if (membership.StartDate.Date < DateTime.Now.Date)
                    throw new ArgumentException($"Дата начала должна быть позже или равна текущей дате. " +
                        $"Введенная дата начала: {membership.StartDate.Date}, текущая дата: {DateTime.Now.Date}");

                var paymentDto = new CreatePaymentDTO
                {
                    Date = DateTime.Now,
                    Price = mt.Price,
                    CashbackPercentage = mt.CashbackPercentage,
                    PaidWithBonuses = membership.PaidWithBonuses,
                    ClientId = membership.ClientId,
                    AdminId = membership.AdminId
                };
                var createdPayment = await _paymentService.AddPaymentAsync(paymentDto);

                var newMembership = new Membership
                {
                    StartDate = membership.StartDate.Date,
                    EndDate = membership.StartDate.AddMonths(mt.Duration).Date,
                    ClientId = membership.ClientId,
                    MembershipTypeId = membership.MembershipTypeId,
                    PaymentId = createdPayment.Id
                };

                await _membershipRep.AddAsync(newMembership);

                await transaction.CommitAsync();

                newMembership = await _membershipRep.GetMembershipByIdAsync(newMembership.Id);

                return new MembershipDTO(newMembership);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<MembershipDTO> UpdateMembershipAsync(MembershipDTO membership)
        {
            var existing = await _membershipRep.GetMembershipByIdAsync(membership.Id) ??
                throw new KeyNotFoundException($"Абонемент с Id {membership.Id} не найден");

            existing.StartDate = membership.StartDate;
            existing.EndDate = membership.EndDate;
            existing.ClientId = membership.ClientId;
            existing.MembershipTypeId = membership.MembershipTypeId;
            existing.PaymentId = membership.PaymentId;

            await _membershipRep.UpdateAsync(existing);

            return new MembershipDTO(existing);
        }

        public async Task DeleteMembership(int id)
        {
            var membership = await _membershipRep.GetMembershipByIdAsync(id) ??
                throw new KeyNotFoundException($"Абонемент с Id {id} не найден");

            await _membershipRep.DeleteAsync(membership);
        }
        public async Task SoftDeleteMembership(int id)
        {
            var membership = await _membershipRep.GetMembershipByIdAsync(id) ??
                throw new KeyNotFoundException($"Абонемент с Id {id} не найден");

            await _membershipRep.SoftDeleteAsync(membership);
        }
    }
}
