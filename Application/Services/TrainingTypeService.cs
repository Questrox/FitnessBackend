using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
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

        public async Task<TrainingTypeDTO> AddTrainingTypeAsync(CreateTrainingTypeDTO dto, IWebHostEnvironment env)
        {
            var type = new TrainingType
            {
                MaxClients = dto.MaxClients,
                Price = dto.Price,
                Name = dto.Name,
                Description = dto.Description,
                CashbackPercentage = dto.CashbackPercentage,
                Duration = dto.Duration,
            };

            if (dto.Image != null && dto.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(env.WebRootPath, "images", "TrainingTypes");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                type.PhotoPath = $"images/TrainingTypes/{uniqueFileName}";
            }
            else
                type.PhotoPath = "";

            await _typeRep.AddAsync(type);
            type = await _typeRep.GetTrainingTypeByIdAsync(type.Id);

            return new TrainingTypeDTO(type);
        }

        public async Task<TrainingTypeDTO> UpdateTrainingTypeAsync(TrainingTypeDTO dto, IWebHostEnvironment env)
        {
            var existing = await _typeRep.GetTrainingTypeByIdAsync(dto.Id) ??
                throw new KeyNotFoundException($"Тип тренировки с Id {dto.Id} не найден");

            existing.MaxClients = dto.MaxClients;
            existing.Price = dto.Price;
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.CashbackPercentage = dto.CashbackPercentage;
            existing.Duration = dto.Duration;

            if (dto.Image != null && dto.Image.Length > 0)
            {
                // Удаляем старое изображение (если есть)
                if (!string.IsNullOrEmpty(existing.PhotoPath))
                {
                    var oldFilePath = Path.Combine(env.WebRootPath, existing.PhotoPath);
                    if (File.Exists(oldFilePath))
                        File.Delete(oldFilePath);
                }

                // Загружаем новое (как в Create)
                var uploadsFolder = Path.Combine(env.WebRootPath, "images", "TrainingTypes");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                existing.PhotoPath = $"images/TrainingTypes/{uniqueFileName}";
            }

            await _typeRep.UpdateAsync(existing);

            return new TrainingTypeDTO(existing);
        }

        public async Task DeleteTrainingType(int id)
        {
            var type = await _typeRep.GetTrainingTypeByIdAsync(id) ??
                throw new KeyNotFoundException($"Тип тренировки с Id {id} не найден");

            await _typeRep.DeleteAsync(type);
        }
        public async Task SoftDeleteTrainingType(int id)
        {
            var type = await _typeRep.GetTrainingTypeByIdAsync(id) ??
                throw new KeyNotFoundException($"Тип тренировки с Id {id} не найден");

            await _typeRep.SoftDeleteAsync(type);
        }
    }
}
