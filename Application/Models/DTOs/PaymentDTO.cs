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
        public PaymentDTO() { }

        public PaymentDTO(Payment p)
        {
            Id = p.Id;
            Date = p.Date;
            Price = p.Price;
            CashbackPercentage = p.CashbackPercentage;
            PaidWithBonuses = p.PaidWithBonuses;


            TrainingReservation = p.TrainingReservation == null ? null : new TrainingReservationDTO(p.TrainingReservation);
        }

        public PaymentDTO(PaymentDTO p)
        {
            Id = p.Id;
            Date = p.Date;
            Price = p.Price;
            CashbackPercentage = p.CashbackPercentage;
            PaidWithBonuses = p.PaidWithBonuses;
            TrainingReservation = p.TrainingReservation;
        }

        public int Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Price { get; set; }

        public int CashbackPercentage { get; set; }

        public decimal PaidWithBonuses { get; set; }

        public virtual TrainingReservationDTO? TrainingReservation { get; set; }
    }
}
