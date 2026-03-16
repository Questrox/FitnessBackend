using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class PaymentDTO
    {
        public PaymentDTO()
        {
            Memberships = new HashSet<MembershipDTO>();
            TrainingReservations = new HashSet<TrainingReservationDTO>();
        }

        public PaymentDTO(Payment p)
        {
            Id = p.Id;
            Date = p.Date;
            Price = p.Price;
            CashbackPercentage = p.CashbackPercentage;
            PaidWithBonuses = p.PaidWithBonuses;

            Memberships = p.Memberships
                .Select(x => new MembershipDTO(x))
                .ToList();

            TrainingReservations = p.TrainingReservations
                .Select(x => new TrainingReservationDTO(x))
                .ToList();
        }

        public PaymentDTO(PaymentDTO p)
        {
            Id = p.Id;
            Date = p.Date;
            Price = p.Price;
            CashbackPercentage = p.CashbackPercentage;
            PaidWithBonuses = p.PaidWithBonuses;
            Memberships = p.Memberships;
            TrainingReservations = p.TrainingReservations;
        }

        public int Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Price { get; set; }

        public int CashbackPercentage { get; set; }

        public decimal PaidWithBonuses { get; set; }

        public virtual ICollection<MembershipDTO> Memberships { get; set; }

        public virtual ICollection<TrainingReservationDTO> TrainingReservations { get; set; }
    }
}
