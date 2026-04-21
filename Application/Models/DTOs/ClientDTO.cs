using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class ClientDTO
    {
        public ClientDTO()
        {
            Memberships = new HashSet<MembershipDTO>();
            TrainingReservations = new HashSet<TrainingReservationDTO>();
        }
        public ClientDTO(Client c)
        {
            Id = c.Id;
            Bonuses = c.Bonuses;
            UserId = c.UserId;
            User = c.User == null ? null : new UserDTO(c.User);
            Memberships = c.Memberships.Select(m => new MembershipDTO(m)).ToList();
            TrainingReservations = c.TrainingReservations.Select(t => new TrainingReservationDTO(t)).ToList();
        }
        public ClientDTO(ClientDTO c)
        {
            Id = c.Id;
            Bonuses = c.Bonuses;
            UserId = c.UserId;
            User = c.User;
            Memberships = c.Memberships;
            TrainingReservations = c.TrainingReservations;
        }
        public int Id { get; set; }
        public decimal Bonuses { get; set; }
        public string? UserId { get; set; }
        public virtual UserDTO? User { get; set; }
        public virtual ICollection<MembershipDTO> Memberships { get; set; }
        public virtual ICollection<TrainingReservationDTO> TrainingReservations { get; set; }
    }
}
