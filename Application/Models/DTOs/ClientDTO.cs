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
            CancellationNotifications = new HashSet<CancellationNotificationDTO>();
        }
        public ClientDTO(Client c)
        {
            Id = c.Id;
            Bonuses = c.Bonuses;
            FullName = c.FullName;
            PhoneNumber = c.PhoneNumber;
            ConfirmationCode = c.ConfirmationCode;
            UserId = c.UserId;
            User = c.User == null ? null : new UserDTO(c.User);
            Memberships = c.Memberships.Select(m => new MembershipDTO(m)).ToList();
            TrainingReservations = c.TrainingReservations.Select(t => new TrainingReservationDTO(t)).ToList();
            CancellationNotifications = c.CancellationNotifications.Select(c => new CancellationNotificationDTO(c)).ToList();
        }
        public ClientDTO(ClientDTO c)
        {
            Id = c.Id;
            Bonuses = c.Bonuses;
            FullName = c.FullName;
            PhoneNumber = c.PhoneNumber;
            ConfirmationCode = c.ConfirmationCode;
            UserId = c.UserId;
            User = c.User;
            Memberships = c.Memberships;
            TrainingReservations = c.TrainingReservations;
            CancellationNotifications = c.CancellationNotifications;
        }
        public int Id { get; set; }
        public decimal Bonuses { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string ConfirmationCode { get; set; } = null!;
        public string? UserId { get; set; }
        public virtual UserDTO? User { get; set; }
        public virtual ICollection<MembershipDTO> Memberships { get; set; }
        public virtual ICollection<TrainingReservationDTO> TrainingReservations { get; set; }
        public virtual ICollection<CancellationNotificationDTO> CancellationNotifications { get; set; }
    }
}
