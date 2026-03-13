using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Client
    {
        public Client()
        {
            Memberships = new HashSet<Membership>();
            TrainingReservations = new HashSet<TrainingReservation>();
            CancellationNotifications = new HashSet<CancellationNotification>();
        }
        [Key]
        public int Id { get; set; }
        public decimal Bonuses { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string ConfirmationCode { get; set; } = null!;
        public string? UserId { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Membership> Memberships { get; set; }
        public virtual ICollection<TrainingReservation> TrainingReservations { get; set; }
        public virtual ICollection<CancellationNotification> CancellationNotifications { get; set; }
    }
}
