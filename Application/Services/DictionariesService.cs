using Domain.Entities;
using Domain.Interfaces;
using Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DictionariesService(IDictionariesRepository _dictionariesRepository)
    {
        public async Task<IEnumerable<ReservationStatusDTO>> GetReservationStatusesAsync()
        {
            var statuses = await _dictionariesRepository.GetReservationStatusesAsync();
            return statuses.Select(s => new ReservationStatusDTO(s));
        }
        public async Task<IEnumerable<TrainingStatusDTO>> GetTrainingStatusesAsync()
        {
            var statuses = await _dictionariesRepository.GetTrainingStatusesAsync();
            return statuses.Select(s => new TrainingStatusDTO(s));
        }
    }
}
