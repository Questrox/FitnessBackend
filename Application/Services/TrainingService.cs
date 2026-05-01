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
    public class TrainingService(ITrainingRepository _trainingRep, ITrainingTypeRepository _typeRep, ICoachRepository _coachRep, 
        IClientRepository _clientRep, ITrainingReservationRepository _reservationRep,
        INotificationRepository _notificationRepository, FitnessDb _db)
    {
        public async Task<string> CheckReservationPossibilityAsync(int trainingId, int? clientId, bool isClient) // ДОБАВИТЬ ПРОВЕРКУ НА АБОНЕМЕНТ
        {
            if (clientId == null)
                throw new ArgumentException("ClientId = null");

            var training = await _trainingRep.GetTrainingByIdAsync(trainingId);
            if (training == null)
                throw new ArgumentException($"Не найдена тренировка с Id {trainingId}");
            if (training.TrainingStatusId == (int)TrainingStatusEnum.Completed)
                return "Тренировка уже была проведена";
            if (training.TrainingStatusId == (int)TrainingStatusEnum.Cancelled)
                return "Тренировка отменена";
            if (DateTime.Now > training.StartDate)
                return "Нельзя записаться на тренировку после ее начала";

            var trainingType = await _typeRep.GetTrainingTypeByIdAsync(training.TrainingTypeId);
            if (training.TrainingReservations.Any(tr => tr.ClientId == clientId && tr.ReservationStatusId != (int)ReservationStatusEnum.Cancelled))
            {
                if (isClient)
                    return "Вы уже записаны";
                else
                    return "Клиент уже записан";
            }
            if (training.TrainingReservations
                .Count(tr => tr.ReservationStatusId != (int)ReservationStatusEnum.Cancelled) >= trainingType.MaxClients)
                return "Нет мест";
            return String.Empty;
        }

        // ТОЖЕ ДОБАВИТЬ ПРОВЕРКУ НА АБОНЕМЕНТ
        public async Task<string> CheckIndividualTrainingCreationPossibilityAsync(CreateIndividualTrainingDTO dto, string userId) 
        {
            var trainingType = await _typeRep.GetTrainingTypeByIdAsync(dto.TrainingTypeId);
            if (trainingType == null)
                return $"Не найден тип тренировки с Id {dto.TrainingTypeId}";
            if (trainingType.MaxClients != 1)
                return "Тренировка не является индивидуальной";

            var client = await _clientRep.GetClientByIdAsync(dto.ClientId);
            if (client == null)
                return $"Не найден клиент с Id {dto.ClientId}";

            var coach = await _coachRep.GetCoachByUserIdAsync(userId);
            if (coach == null)
                return $"Не найден тренер";

            if (DateTime.Now > dto.StartDate)
                return "Нельзя создать тренировку в прошлом";
            bool foundSchedule = false;
            DayOfWeek day = dto.StartDate.DayOfWeek;
            DateTime endDate = dto.StartDate.AddMinutes(trainingType.Duration);
            foreach (var cs in coach.CoachSchedules)
            {
                if (cs.WeekDay == day && cs.StartTime <= dto.StartDate.TimeOfDay && cs.EndTime >= endDate.TimeOfDay)
                {
                    foundSchedule = true; 
                    break;
                }
            }
            if (!foundSchedule)
                return "Данная тренировка выходит за границы вашего рабочего времени в этот день";

            if (coach.Trainings.Any(t => t.TrainingStatusId != (int)TrainingStatusEnum.Cancelled &&
                                    t.StartDate < endDate && t.EndDate > dto.StartDate))
                return "У вас уже есть тренировка в это время";

            if (client.TrainingReservations.Any(res => res.ReservationStatusId != (int)ReservationStatusEnum.Cancelled &&
                                                res.Training.StartDate < endDate && res.Training.EndDate > dto.StartDate))
                return "Клиент уже записан на тренировку в это время";

            return String.Empty;
        }
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
            if (dto.StartDate.ToLocalTime() < DateTime.Now)
                throw new ArgumentException($"Нельзя добавить тренировку раньше текущей даты и времени");
            var type = await _typeRep.GetTrainingTypeByIdAsync(dto.TrainingTypeId);
            if (type == null)
                throw new ArgumentException($"Не найден тип тренировки с Id {dto.TrainingTypeId}");
            var coach = await _coachRep.GetCoachByIdAsync(dto.CoachId);
            if (coach == null)
                throw new ArgumentException($"Не найден тренер с Id {dto.CoachId}");
            var availableCoaches = await _coachRep.GetAvailableCoachesAsync(dto.StartDate.ToLocalTime(),
                dto.StartDate.ToLocalTime().AddMinutes(type.Duration));
            if (!availableCoaches.Contains(coach))
                throw new ArgumentException($"Тренер с Id {dto.CoachId} занят в данный временной промежуток");

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var training = new Training
                {
                    StartDate = dto.StartDate.ToLocalTime(),
                    EndDate = dto.StartDate.ToLocalTime().AddMinutes(type.Duration),
                    Price = type.Price,
                    CashbackPercentage = type.CashbackPercentage,
                    CoachId = dto.CoachId,
                    TrainingTypeId = dto.TrainingTypeId,
                    TrainingStatusId = (int)TrainingStatusEnum.Pending,
                };

                await _trainingRep.AddAsync(training);
                training = await _trainingRep.GetTrainingByIdAsync(training.Id);
                await transaction.CommitAsync();

                return new TrainingDTO(training);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<TrainingDTO> AddIndividualTrainingAsync(CreateIndividualTrainingDTO dto, string userId)
        {
            string checkResult = await CheckIndividualTrainingCreationPossibilityAsync(dto, userId);
            if (checkResult != "")
                throw new ArgumentException(checkResult);
            var type = await _typeRep.GetTrainingTypeByIdAsync(dto.TrainingTypeId);
            var coach = await _coachRep.GetCoachByUserIdAsync(userId);
            var training = new Training
            {
                StartDate = dto.StartDate.ToLocalTime(),
                EndDate = dto.StartDate.ToLocalTime().AddMinutes(type.Duration),
                Price = type.Price,
                CashbackPercentage = type.CashbackPercentage,
                CoachId = coach.Id,
                TrainingTypeId = dto.TrainingTypeId,
                TrainingStatusId = (int)TrainingStatusEnum.Pending,
            };

            await _trainingRep.AddAsync(training);

            var reservation = new TrainingReservation
            {
                ClientId = dto.ClientId,
                TrainingId = training.Id,
                ReservationStatusId = (int)ReservationStatusEnum.Pending,
            };
            await _reservationRep.AddAsync(reservation);

            training = await _trainingRep.GetTrainingByIdAsync(training.Id);

            return new TrainingDTO(training);
        }

        public async Task<TrainingDTO> UpdateTrainingAsync(TrainingDTO dto)
        {
            var existing = await _trainingRep.GetTrainingByIdAsync(dto.Id) ??
                throw new KeyNotFoundException($"Тренировка с Id {dto.Id} не найдена");

            existing.StartDate = dto.StartDate;
            existing.EndDate = dto.EndDate;
            existing.CoachId = dto.CoachId;
            existing.TrainingTypeId = dto.TrainingTypeId;

            await _trainingRep.UpdateAsync(existing);

            return new TrainingDTO(existing);
        }

        public async Task<TrainingDTO> CancelTrainingAsync(int id)
        {
            var existing = await _trainingRep.GetTrainingByIdAsync(id) ??
                throw new KeyNotFoundException($"Тренировка с Id {id} не найдена");

            if (existing.TrainingStatusId == (int)TrainingStatusEnum.Completed)
                throw new ArgumentException("Нельзя отменить уже проведенную тренировку");
            if (existing.TrainingStatusId == (int)TrainingStatusEnum.Cancelled)
                throw new ArgumentException("Тренировка уже отменена");
            if (existing.StartDate <= DateTime.Now)
                throw new ArgumentException("Тренировка уже началась, ее нельзя отменить");

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // меняем статус тренировки
                existing.TrainingStatusId = (int)TrainingStatusEnum.Cancelled;
                await _trainingRep.UpdateAsync(existing);

                // отменяем записи
                var reservationsToUpdate = existing.TrainingReservations
                    .Where(r => r.ReservationStatusId != (int)ReservationStatusEnum.Cancelled)
                    .ToList();

                foreach (var reservation in reservationsToUpdate)
                {
                    reservation.ReservationStatusId = (int)ReservationStatusEnum.Cancelled;
                }

                if (reservationsToUpdate.Any())
                {
                    await _db.SaveChangesAsync();
                }

                // создаем уведомления для активных записей
                List<CancellationNotification> notifications = new List<CancellationNotification>();
                foreach (var res in reservationsToUpdate)
                {
                    CancellationNotification notification = new CancellationNotification
                    {
                        TrainingId = id,
                        ClientId = res.ClientId,
                        IsNotified = false
                    };
                    notifications.Add(notification);
                }
                if (notifications.Count > 0)
                    await _notificationRepository.AddRangeAsync(notifications);

                await transaction.CommitAsync();

                var updated = await _trainingRep.GetTrainingByIdAsync(existing.Id);
                return new TrainingDTO(updated);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<TrainingWithNotificationsDTO>> GetTrainingsWithNotificationsAsync()
        {
            var trainings = await _trainingRep.GetTrainingsWithNotificationsAsync();
            return trainings.Select(t => new TrainingWithNotificationsDTO(t)).ToList();
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
