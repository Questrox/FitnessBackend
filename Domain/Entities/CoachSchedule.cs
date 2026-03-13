using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CoachSchedule
    {
        [Key]
        public int Id { get; set; }
        public DayOfWeek WeekDay { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int CoachId { get; set; }
        public virtual Coach? Coach { get; set; }
    }
}
