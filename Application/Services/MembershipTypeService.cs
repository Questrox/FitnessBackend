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
    public class MembershipTypeService(IMembershipTypeRepository _typeRep)
    {
        public async Task<IEnumerable<MembershipTypeDTO>> GetMembershipTypesAsync()
        {
            var types = await _typeRep.GetMembershipTypesAsync();
            return types.Select(t => new MembershipTypeDTO(t));
        }

        public async Task<MembershipTypeDTO?> GetMembershipTypeByIdAsync(int id)
        {
            var type = await _typeRep.GetMembershipTypeById(id);
            return type == null ? null : new MembershipTypeDTO(type);
        }

        public async Task<MembershipTypeDTO> AddMembershipTypeAsync(CreateMembershipTypeDTO type)
        {
            var newType = new MembershipType
            {
                Name = type.Name,
                Description = type.Description,
                Price = type.Price,
                CashbackPercentage = type.CashbackPercentage,
                Duration = type.Duration
            };

            await _typeRep.AddAsync(newType);
            newType = await _typeRep.GetMembershipTypeById(newType.Id);

            return new MembershipTypeDTO(newType);
        }

        public async Task<MembershipTypeDTO> UpdateMembershipTypeAsync(MembershipTypeDTO type)
        {
            var existing = await _typeRep.GetMembershipTypeById(type.Id) ??
                throw new KeyNotFoundException($"Тип абонемента с Id {type.Id} не найден");

            existing.Name = type.Name;
            existing.Description = type.Description;
            existing.Price = type.Price;
            existing.CashbackPercentage = type.CashbackPercentage;
            existing.Duration = type.Duration;

            await _typeRep.UpdateAsync(existing);

            return new MembershipTypeDTO(existing);
        }

        public async Task DeleteMembershipType(int id)
        {
            var type = await _typeRep.GetMembershipTypeById(id) ??
                throw new KeyNotFoundException($"Тип абонемента с Id {id} не найден");

            await _typeRep.DeleteAsync(type);
        }
        public async Task SoftDeleteMembershipType(int id)
        {
            var type = await _typeRep.GetMembershipTypeById(id) ??
                throw new KeyNotFoundException($"Тип абонемента с Id {id} не найден");

            await _typeRep.SoftDeleteAsync(type);
        }
    }
}
