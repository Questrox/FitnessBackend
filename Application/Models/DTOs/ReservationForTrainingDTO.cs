using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class ReservationForTrainingDTO
    {
        public ReservationForTrainingDTO() { }
        public ReservationForTrainingDTO(TrainingReservation res)
        {
            Id = res.Id;
            ReservationStatusId = res.ReservationStatusId;
            ReservationStatus = res.ReservationStatus != null ? new ReservationStatusDTO(res.ReservationStatus) : null;
            if (res.Client != null)
                User = res.Client.User != null ? new UserDTO(res.Client.User) : null;
            else
                User = null;
        }
        public ReservationForTrainingDTO(ReservationForTrainingDTO dto)
        {
            Id = dto.Id;
            ReservationStatusId = dto.ReservationStatusId;
            ReservationStatus = dto.ReservationStatus;
            User = dto.User;
        }
        public int Id { get; set; }
        public int ReservationStatusId { get; set; }
        public virtual ReservationStatusDTO? ReservationStatus { get; set; }
        public virtual UserDTO? User { get; set; }
    }
}
