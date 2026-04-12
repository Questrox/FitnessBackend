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
        public ReservationStatusDTO() { }

        public ReservationStatusDTO(ReservationStatus r)
        {
            Id = r.Id;
            Name = r.Name;
        }

        public ReservationStatusDTO(ReservationStatusDTO r)
        {
            Id = r.Id;
            Name = r.Name;
        }

        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }
}
