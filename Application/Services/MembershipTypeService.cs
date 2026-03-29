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
            if (type.Price <= 0)
                throw new ArgumentException($"Цена должна быть положительной. Текущая цена: {type.Price}");
            if (type.CashbackPercentage < 0 || type.CashbackPercentage > 100)
                throw new ArgumentException($"Процент кэшбека должен быть в пределах от 0 до 100. Текущий процент: {type.CashbackPercentage}");
            if (type.Duration <= 0)
                throw new ArgumentException($"Продолжительность в месяцах должна быть положительной. Текущая продолжительность: {type.Duration}");
            if (type.Name.Length == 0)
                throw new ArgumentException("Необходимо ввести название типа абонемента");
            if (type.Description.Length == 0)
                throw new ArgumentException("Необходимо ввести описание типа абонемента");
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

            if (type.Price <= 0)
                throw new ArgumentException($"Цена должна быть положительной. Текущая цена: {type.Price}");
            if (type.CashbackPercentage < 0 || type.CashbackPercentage > 100)
                throw new ArgumentException($"Процент кэшбека должен быть в пределах от 0 до 100. Текущий процент: {type.CashbackPercentage}");
            if (type.Duration <= 0)
                throw new ArgumentException($"Продолжительность в месяцах должна быть положительной. Текущая продолжительность: {type.Duration}");
            if (type.Name.Length == 0)
                throw new ArgumentException("Необходимо ввести название типа абонемента");
            if (type.Description.Length == 0)
                throw new ArgumentException("Необходимо ввести описание типа абонемента");

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
