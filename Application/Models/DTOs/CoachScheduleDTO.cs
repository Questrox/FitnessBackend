using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class CoachScheduleDTO
    {
        public CoachScheduleDTO() { }

        public CoachScheduleDTO(CoachSchedule c)
        {
            Id = c.Id;
            WeekDay = c.WeekDay;
            StartTime = c.StartTime;
            EndTime = c.EndTime;
            CoachId = c.CoachId;
            Coach = c.Coach == null ? null : new CoachDTO(c.Coach);
        }

        public CoachScheduleDTO(CoachScheduleDTO c)
        {
            Id = c.Id;
            WeekDay = c.WeekDay;
            StartTime = c.StartTime;
            EndTime = c.EndTime;
            CoachId = c.CoachId;
            Coach = c.Coach;
        }

        public int Id { get; set; }

        public DayOfWeek WeekDay { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int CoachId { get; set; }

        public virtual CoachDTO? Coach { get; set; }
    }
}
