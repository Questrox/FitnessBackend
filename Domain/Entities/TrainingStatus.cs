using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum TrainingStatusEnum
    {
        Pending = 1,
        Completed = 2,
        Cancelled = 3
    }
    public class TrainingStatus
    {
        public TrainingStatus()
        {
            Trainings = new HashSet<Training>();
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<Training> Trainings { get; set; }
    }
}
