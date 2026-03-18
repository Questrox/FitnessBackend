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
    public class TrainingTypeService(ITrainingTypeRepository _typeRep)
    {
        public async Task<IEnumerable<TrainingTypeDTO>> GetTrainingTypesAsync()
        {
            var types = await _typeRep.GetTrainingTypesAsync();
            return types.Select(t => new TrainingTypeDTO(t));
        }

        public async Task<TrainingTypeDTO?> GetTrainingTypeByIdAsync(int id)
        {
            var type = await _typeRep.GetTrainingTypeByIdAsync(id);
            return type == null ? null : new TrainingTypeDTO(type);
        }

        public async Task<TrainingTypeDTO> AddTrainingTypeAsync(CreateTrainingTypeDTO dto)
        {
            var type = new TrainingType
            {
                Price = dto.Price,
                Name = dto.Name,
                Description = dto.Description,
                CashbackPercentage = dto.CashbackPercentage
            };

            await _typeRep.AddAsync(type);
            type = await _typeRep.GetTrainingTypeByIdAsync(type.Id);

            return new TrainingTypeDTO(type);
        }

        public async Task<TrainingTypeDTO> UpdateTrainingTypeAsync(TrainingTypeDTO dto)
        {
            var existing = await _typeRep.GetTrainingTypeByIdAsync(dto.Id) ??
                throw new KeyNotFoundException($"Тип тренировки с Id {dto.Id} не найден");

            existing.Price = dto.Price;
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.CashbackPercentage = dto.CashbackPercentage;

            await _typeRep.UpdateAsync(existing);

            return new TrainingTypeDTO(existing);
        }

        public async Task DeleteTrainingType(int id)
        {
            var type = await _typeRep.GetTrainingTypeByIdAsync(id) ??
                throw new KeyNotFoundException($"Тип тренировки с Id {id} не найден");

            await _typeRep.DeleteAsync(type);
        }
    }
}
