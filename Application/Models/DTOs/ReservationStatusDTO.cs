using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class ReservationStatusDTO
    {
        public ReservationStatusDTO()
        {
            TrainingReservations = new HashSet<TrainingReservationDTO>();
        }

        public ReservationStatusDTO(ReservationStatus r)
        {
            Id = r.Id;
            Name = r.Name;

            TrainingReservations = r.TrainingReservations
                .Select(x => new TrainingReservationDTO(x))
                .ToList();
        }

        public ReservationStatusDTO(ReservationStatusDTO r)
        {
            Id = r.Id;
            Name = r.Name;
            TrainingReservations = r.TrainingReservations;
        }

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<TrainingReservationDTO> TrainingReservations { get; set; }
    }
}
