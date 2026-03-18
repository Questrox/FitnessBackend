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
    public class CoachService(ICoachRepository _coachRep)
    {
        public async Task<IEnumerable<CoachDTO>> GetCoachesAsync()
        {
            var coaches = await _coachRep.GetCoachesAsync();
            return coaches.Select(c => new CoachDTO(c));
        }

        public async Task<CoachDTO?> GetCoachByIdAsync(int id)
        {
            var coach = await _coachRep.GetCoachByIdAsync(id);
            return coach == null ? null : new CoachDTO(coach);
        }

        public async Task<CoachDTO> AddCoachAsync(CreateCoachDTO coach)
        {
            var newCoach = new Coach
            {
                Experience = coach.Experience,
                PhotoPath = coach.PhotoPath,
                UserId = coach.UserId
            };

            await _coachRep.AddAsync(newCoach);
            newCoach = await _coachRep.GetCoachByIdAsync(newCoach.Id);

            return new CoachDTO(newCoach);
        }

        public async Task<CoachDTO> UpdateCoachAsync(CoachDTO coach)
        {
            var existingCoach = await _coachRep.GetCoachByIdAsync(coach.Id) ??
                throw new KeyNotFoundException($"Тренер с Id {coach.Id} не найден");

            existingCoach.Experience = coach.Experience;
            existingCoach.PhotoPath = coach.PhotoPath;
            existingCoach.UserId = coach.UserId;

            await _coachRep.UpdateAsync(existingCoach);

            return new CoachDTO(existingCoach);
        }

        public async Task DeleteCoach(int coachId)
        {
            var coach = await _coachRep.GetCoachByIdAsync(coachId) ??
                throw new KeyNotFoundException($"Тренер с Id {coachId} не найден");

            await _coachRep.DeleteAsync(coach);
        }

        public async Task SoftDeleteCoach(int coachId)
        {
            var coach = await _coachRep.GetCoachByIdAsync(coachId) ??
                throw new KeyNotFoundException($"Тренер с Id {coachId} не найден");

            await _coachRep.SoftDeleteAsync(coach);
        }
    }
}
