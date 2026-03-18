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
    public class TrainingReservationService(ITrainingReservationRepository _reservationRep)
    {
        public async Task<IEnumerable<TrainingReservationDTO>> GetClientReservationsAsync(int clientId)
        {
            var reservations = await _reservationRep.GetClientReservations(clientId);
            return reservations.Select(r => new TrainingReservationDTO(r));
        }

        public async Task<TrainingReservationDTO?> GetReservationByIdAsync(int id)
        {
            var reservation = await _reservationRep.GetReservationById(id);
            return reservation == null ? null : new TrainingReservationDTO(reservation);
        }

        public async Task<TrainingReservationDTO> AddReservationAsync(CreateTrainingReservationDTO dto)
        {
            var reservation = new TrainingReservation
            {
                ClientId = dto.ClientId,
                TrainingId = dto.TrainingId
            };

            await _reservationRep.AddAsync(reservation);
            reservation = await _reservationRep.GetReservationById(reservation.Id);

            return new TrainingReservationDTO(reservation);
        }

        public async Task<TrainingReservationDTO> UpdateReservationAsync(TrainingReservationDTO dto)
        {
            var existing = await _reservationRep.GetReservationById(dto.Id) ??
                throw new KeyNotFoundException($"Запись с Id {dto.Id} не найдена");

            existing.ClientId = dto.ClientId;
            existing.TrainingId = dto.TrainingId;

            await _reservationRep.UpdateAsync(existing);

            return new TrainingReservationDTO(existing);
        }

        public async Task DeleteReservation(int id)
        {
            var reservation = await _reservationRep.GetReservationById(id) ??
                throw new KeyNotFoundException($"Запись с Id {id} не найдена");

            await _reservationRep.DeleteAsync(reservation);
        }
    }
}
