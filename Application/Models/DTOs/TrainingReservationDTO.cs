using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class TrainingReservationDTO
    {
        public TrainingReservationDTO() { }

        public TrainingReservationDTO(TrainingReservation r)
        {
            Id = r.Id;
            ClientId = r.ClientId;
            TrainingId = r.TrainingId;
            PaymentId = r.PaymentId;
            ReservationStatusId = r.ReservationStatusId;

            Training = r.Training == null ? null : new TrainingDTO(r.Training);
            Payment = r.Payment == null ? null : new PaymentDTO(r.Payment);
            ReservationStatus = r.ReservationStatus == null
                ? null
                : new ReservationStatusDTO(r.ReservationStatus);
        }

        public TrainingReservationDTO(TrainingReservationDTO r)
        {
            Id = r.Id;
            ClientId = r.ClientId;
            TrainingId = r.TrainingId;
            PaymentId = r.PaymentId;
            ReservationStatusId = r.ReservationStatusId;
            Training = r.Training;
            Payment = r.Payment;
            ReservationStatus = r.ReservationStatus;
        }

        public int Id { get; set; }

        public int ClientId { get; set; }

        public int TrainingId { get; set; }

        public int? PaymentId { get; set; }

        public int ReservationStatusId { get; set; }

        public virtual TrainingDTO? Training { get; set; }

        public virtual PaymentDTO? Payment { get; set; }

        public virtual ReservationStatusDTO? ReservationStatus { get; set; }
    }
}
