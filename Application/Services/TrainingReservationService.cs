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
    public class TrainingReservationService(IUnitOfWork _uow, ITrainingReservationRepository _reservationRep, ICoachRepository _coachRep, 
        TrainingService _trainingService, PaymentService _paymentService)
    {
        public async Task<IEnumerable<TrainingReservationDTO>> GetClientReservationsAsync(int clientId)
        {
            var reservations = await _reservationRep.GetClientReservationsAsync(clientId);
            return reservations.Select(r => new TrainingReservationDTO(r));
        }
        
        public async Task<IEnumerable<ReservationForTrainingDTO>> GetReservationsByTrainingIdAsync(int trainingId)
        {
            var reservations = await _reservationRep.GetReservationsByTrainingIdAsync(trainingId);
            return reservations.Select(r => new ReservationForTrainingDTO(r)).ToList();
        }

        public async Task<ReservationForTrainingDTO> ConfirmTrainingAttendanceAsync(int resId, string userId)
        {
            var res = await _reservationRep.GetReservationByIdAsync(resId);
            var coach = await _coachRep.GetCoachByUserIdAsync(userId);
            if (coach == null)
                throw new ArgumentException("Не найден тренер");
            if (res == null)
                throw new ArgumentException($"Запись с Id {resId} не найдена");
            if (res.Training.CoachId != coach.Id)
                throw new ArgumentException("Подтвердить посещение может только ответственный за тренировку тренер");
            if (res.ReservationStatusId == (int)ReservationStatusEnum.Visited)
                throw new ArgumentException("Посещение уже подтверждено");
            if (res.ReservationStatusId == (int)ReservationStatusEnum.Cancelled)
                throw new ArgumentException("Запись отменена");

            if (res.Training.TrainingStatusId == (int)TrainingStatusEnum.Cancelled)
                throw new ArgumentException("Нельзя подтвердить посещение у отмененной тренировки");
            if (res.Training.TrainingStatusId == (int)TrainingStatusEnum.Completed)
                throw new ArgumentException("Тренировка уже проведена");

            res.ReservationStatusId = (int)ReservationStatusEnum.Visited;
            await _reservationRep.UpdateAsync(res);

            var result = await _reservationRep.GetReservationByIdAsync(resId);
            return new ReservationForTrainingDTO(result);
        }

        public async Task<TrainingReservationDTO?> GetReservationByIdAsync(int id)
        {
            var reservation = await _reservationRep.GetReservationByIdAsync(id);
            return reservation == null ? null : new TrainingReservationDTO(reservation);
        }

        public async Task<TrainingReservationDTO> AddReservationAsync(CreateTrainingReservationDTO dto, bool isClient)
        {
            if (dto.ClientId == null)
                throw new ArgumentException("ClientId = null");
            string error = await _trainingService.CheckReservationPossibilityAsync(dto.TrainingId, dto.ClientId, isClient);
            if (error != String.Empty)
                throw new ArgumentException(error);
            var reservation = new TrainingReservation
            {
                ClientId = (int)dto.ClientId,
                TrainingId = dto.TrainingId,
                ReservationStatusId = (int)ReservationStatusEnum.Pending
            };
            await _reservationRep.AddAsync(reservation);
            reservation = await _reservationRep.GetReservationByIdAsync(reservation.Id);

            return new TrainingReservationDTO(reservation);
        }

        public async Task<TrainingReservationDTO> UpdateReservationAsync(TrainingReservationDTO dto)
        {
            var existing = await _reservationRep.GetReservationByIdAsync(dto.Id) ??
                throw new KeyNotFoundException($"Запись с Id {dto.Id} не найдена");

            existing.ClientId = dto.ClientId;
            existing.TrainingId = dto.TrainingId;
            existing.PaymentId = dto.PaymentId;
            existing.ReservationStatusId = dto.ReservationStatusId;

            await _reservationRep.UpdateAsync(existing);

            return new TrainingReservationDTO(existing);
        }

        public async Task<TrainingReservationDTO> CancelReservationAsync(int id)
        {
            var existing = await _reservationRep.GetReservationByIdAsync(id) ??
                throw new KeyNotFoundException($"Запись с Id {id} не найдена");
            if (existing.ReservationStatusId != (int)ReservationStatusEnum.Pending)
                throw new ArgumentException("Можно отменить только запись со статусом \"Ожидание\"");

            if (existing.Training.TrainingType.MaxClients == 1)
                await _trainingService.CancelTrainingAsync(existing.Training.Id, "", false);
            else
            {
                existing.ReservationStatusId = (int)ReservationStatusEnum.Cancelled;
                await _reservationRep.UpdateAsync(existing);
            }
            var updated = await _reservationRep.GetReservationByIdAsync(id);
            return new TrainingReservationDTO(updated);
        }

        public async Task<TrainingReservationDTO> ConfirmPaymentAsync(int reservationId, CreatePaymentDTO dto)
        {
            var reservation = await _reservationRep.GetReservationByIdAsync(reservationId);
            if (reservation == null)
                throw new ArgumentException($"Не найдена запись с Id {reservationId}");
            if (reservation.ReservationStatusId == (int)ReservationStatusEnum.Pending)
                throw new ArgumentException("Нельзя совершить оплату, т.к. тренировка еще не посещена");
            if (reservation.ReservationStatusId == (int)ReservationStatusEnum.Paid)
                throw new ArgumentException("Оплата уже была произведена");
            if (reservation.ReservationStatusId == (int)ReservationStatusEnum.Cancelled)
                throw new ArgumentException("Нельзя совершить оплату отмененной записи");

            await using var transaction = await _uow.BeginTransactionAsync();

            try
            {
                var payment = await _paymentService.AddPaymentAsync(dto);
                reservation.PaymentId = payment.Id;
                reservation.ReservationStatusId = (int)ReservationStatusEnum.Paid;
                await _reservationRep.UpdateAsync(reservation);
                var updated = await _reservationRep.GetReservationByIdAsync(reservationId);
                await transaction.CommitAsync();
                return new TrainingReservationDTO(updated);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteReservation(int id)
        {
            var reservation = await _reservationRep.GetReservationByIdAsync(id) ??
                throw new KeyNotFoundException($"Запись с Id {id} не найдена");

            await _reservationRep.DeleteAsync(reservation);
        }
        public async Task SoftDeleteReservation(int id)
        {
            var reservation = await _reservationRep.GetReservationByIdAsync(id) ??
                throw new KeyNotFoundException($"Запись с Id {id} не найдена");

            await _reservationRep.SoftDeleteAsync(reservation);
        }
    }
}
