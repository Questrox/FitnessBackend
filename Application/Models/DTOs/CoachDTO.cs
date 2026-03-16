using Application.Models.Old;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class CoachDTO
    {
        public CoachDTO()
        {
            CoachSchedules = new HashSet<CoachScheduleDTO>();
            Trainings = new HashSet<TrainingDTO>();
        }

        public CoachDTO(Coach c)
        {
            Id = c.Id;
            Experience = c.Experience;
            PhotoPath = c.PhotoPath;
            UserId = c.UserId;

            User = c.User == null ? null : new UserDTO(c.User);

            CoachSchedules = c.CoachSchedules
                .Select(s => new CoachScheduleDTO(s))
                .ToList();

            Trainings = c.Trainings
                .Select(t => new TrainingDTO(t))
                .ToList();
        }

        public CoachDTO(CoachDTO c)
        {
            Id = c.Id;
            Experience = c.Experience;
            PhotoPath = c.PhotoPath;
            UserId = c.UserId;
            User = c.User;
            CoachSchedules = c.CoachSchedules;
            Trainings = c.Trainings;
        }

        public int Id { get; set; }

        public int Experience { get; set; }

        public string? PhotoPath { get; set; }

        public string? UserId { get; set; }

        public virtual UserDTO? User { get; set; }

        public virtual ICollection<CoachScheduleDTO> CoachSchedules { get; set; }

        public virtual ICollection<TrainingDTO> Trainings { get; set; }
    }
}
