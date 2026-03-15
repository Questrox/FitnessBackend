using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Coach : ISoftDeletable
    {
        public Coach()
        {
            CoachSchedules = new HashSet<CoachSchedule>();
            Trainings = new HashSet<Training>();
        }
        [Key]
        public int Id { get; set; }
        public int Experience { get; set; }
        public string? PhotoPath { get; set; }
        public string? UserId { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<CoachSchedule> CoachSchedules { get; set; }
        public virtual ICollection<Training> Trainings { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}