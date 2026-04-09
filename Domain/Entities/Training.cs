using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Training : ISoftDeletable
    {
        public Training()
        {
            TrainingReservations = new HashSet<TrainingReservation>();
            CancellationNotifications = new HashSet<CancellationNotification>();
        }
        [Key]
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public int CashbackPercentage { get; set; }
        public int CoachId { get; set; }
        public int TrainingTypeId { get; set; }
        public int TrainingStatusId { get; set; }
        public virtual Coach? Coach { get; set; }
        public virtual TrainingType? TrainingType { get; set; }
        public virtual TrainingStatus? TrainingStatus { get; set; }
        public virtual ICollection<TrainingReservation> TrainingReservations { get; set; }
        public virtual ICollection<CancellationNotification> CancellationNotifications { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
