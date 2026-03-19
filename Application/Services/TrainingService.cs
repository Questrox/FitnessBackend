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
    public class TrainingService(ITrainingRepository _trainingRep)
    {
        public async Task<IEnumerable<TrainingDTO>> GetTrainingsForPeriodAsync(DateTime start, DateTime end)
        {
            var trainings = await _trainingRep.GetTrainingsForPeriodAsync(start, end);
            return trainings.Select(t => new TrainingDTO(t));
        }

        public async Task<TrainingDTO?> GetTrainingByIdAsync(int id)
        {
            var training = await _trainingRep.GetTrainingByIdAsync(id);
            return training == null ? null : new TrainingDTO(training);
        }

        public async Task<TrainingDTO> AddTrainingAsync(CreateTrainingDTO dto)
        {
            var training = new Training
            {
                Date = dto.Date,
                CoachId = dto.CoachId,
                TrainingTypeId = dto.TrainingTypeId
            };

            await _trainingRep.AddAsync(training);
            training = await _trainingRep.GetTrainingByIdAsync(training.Id);

            return new TrainingDTO(training);
        }

        public async Task<TrainingDTO> UpdateTrainingAsync(TrainingDTO dto)
        {
            var existing = await _trainingRep.GetTrainingByIdAsync(dto.Id) ??
                throw new KeyNotFoundException($"Тренировка с Id {dto.Id} не найдена");

            existing.Date = dto.Date;
            existing.CoachId = dto.CoachId;
            existing.TrainingTypeId = dto.TrainingTypeId;

            await _trainingRep.UpdateAsync(existing);

            return new TrainingDTO(existing);
        }

        public async Task DeleteTraining(int id)
        {
            var training = await _trainingRep.GetTrainingByIdAsync(id) ??
                throw new KeyNotFoundException($"Тренировка с Id {id} не найдена");

            await _trainingRep.DeleteAsync(training);
        }
        public async Task SoftDeleteTraining(int id)
        {
            var training = await _trainingRep.GetTrainingByIdAsync(id) ??
                throw new KeyNotFoundException($"Тренировка с Id {id} не найдена");

            await _trainingRep.SoftDeleteAsync(training);
        }
    }
}
