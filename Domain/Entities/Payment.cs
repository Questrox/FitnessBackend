using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Payment
    {
        public Payment()
        {
            Memberships = new HashSet<Membership>();
            TrainingReservations = new HashSet<TrainingReservation>();
        }
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public int CashbackPercentage { get; set; }
        public decimal PaidWithBonuses { get; set; }
        public virtual ICollection<Membership> Memberships { get; set; }
        public virtual ICollection<TrainingReservation> TrainingReservations { get; set; }
    }
}
