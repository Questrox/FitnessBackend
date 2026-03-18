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
    public class MembershipService(IMembershipRepository _membershipRep)
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
            var newMembership = new Membership
            {
                StartDate = membership.StartDate,
                EndDate = membership.EndDate,
                ClientId = membership.ClientId,
                MembershipTypeId = membership.MembershipTypeId,
                PaymentId = membership.PaymentId
            };

            await _membershipRep.AddAsync(newMembership);
            newMembership = await _membershipRep.GetMembershipByIdAsync(newMembership.Id);

            return new MembershipDTO(newMembership);
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
