using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum ReservationStatusEnum
    {
        Pending = 1,
        Visited = 2,
        Paid = 3,
        Cancelled = 4
    }
    public class ReservationStatus
    {
        public ReservationStatus()
        {
            TrainingReservations = new HashSet<TrainingReservation>();
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<TrainingReservation> TrainingReservations { get; set; }
    }
}
