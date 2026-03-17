using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Payment : ISoftDeletable
    {
        public Payment() { }
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public int CashbackPercentage { get; set; }
        public decimal PaidWithBonuses { get; set; }
        public virtual Membership? Membership { get; set; }
        public virtual TrainingReservation? TrainingReservation { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
