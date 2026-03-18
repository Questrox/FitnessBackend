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
    public class CoachScheduleService(ICoachScheduleRepository _scheduleRep)
    {
        public async Task<IEnumerable<CoachScheduleDTO>> GetScheduleForCoachAsync(int coachId)
        {
            var schedules = await _scheduleRep.GetScheduleForCoachAsync(coachId);
            return schedules.Select(s => new CoachScheduleDTO(s));
        }

        public async Task<CoachScheduleDTO?> GetCoachScheduleByIdAsync(int id)
        {
            var schedule = await _scheduleRep.GetCoachScheduleByIdAsync(id);
            return schedule == null ? null : new CoachScheduleDTO(schedule);
        }

        public async Task<CoachScheduleDTO> AddCoachScheduleAsync(CreateCoachScheduleDTO schedule)
        {
            var newSchedule = new CoachSchedule
            {
                WeekDay = schedule.WeekDay,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                CoachId = schedule.CoachId
            };

            await _scheduleRep.AddAsync(newSchedule);
            newSchedule = await _scheduleRep.GetCoachScheduleByIdAsync(newSchedule.Id);

            return new CoachScheduleDTO(newSchedule);
        }

        public async Task<CoachScheduleDTO> UpdateCoachScheduleAsync(CoachScheduleDTO schedule)
        {
            var existing = await _scheduleRep.GetCoachScheduleByIdAsync(schedule.Id) ??
                throw new KeyNotFoundException($"Расписание с Id {schedule.Id} не найдено");

            existing.WeekDay = schedule.WeekDay;
            existing.StartTime = schedule.StartTime;
            existing.EndTime = schedule.EndTime;
            existing.CoachId = schedule.CoachId;

            await _scheduleRep.UpdateAsync(existing);

            return new CoachScheduleDTO(existing);
        }

        public async Task DeleteCoachSchedule(int id)
        {
            var schedule = await _scheduleRep.GetCoachScheduleByIdAsync(id) ??
                throw new KeyNotFoundException($"Расписание с Id {id} не найдено");

            await _scheduleRep.DeleteAsync(schedule);
        }
        public async Task SoftDeleteCoachSchedule(int id)
        {
            var schedule = await _scheduleRep.GetCoachScheduleByIdAsync(id) ??
                throw new KeyNotFoundException($"Расписание с Id {id} не найдено");

            await _scheduleRep.SoftDeleteAsync(schedule);
        }
    }
}
