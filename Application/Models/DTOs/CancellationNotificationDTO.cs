using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class CancellationNotificationDTO
    {
        public CancellationNotificationDTO() { }

        public CancellationNotificationDTO(CancellationNotification c)
        {
            Id = c.Id;
            IsNotified = c.IsNotified;
            TrainingId = c.TrainingId;
            Training = c.Training == null ? null : new TrainingDTO(c.Training);
            ClientId = c.ClientId;
            Client = c.Client == null ? null : new ClientDTO(c.Client);
            AdminId = c.AdminId;
            Admin = c.Admin == null ? null : new UserDTO(c.Admin);
        }

        public CancellationNotificationDTO(CancellationNotificationDTO c)
        {
            Id = c.Id;
            IsNotified = c.IsNotified;
            TrainingId = c.TrainingId;
            Training = c.Training;
            ClientId = c.ClientId;
            Client = c.Client;
            AdminId = c.AdminId;
            Admin = c.Admin;
        }

        public int Id { get; set; }

        public bool IsNotified { get; set; }

        public int TrainingId { get; set; }

        public virtual TrainingDTO? Training { get; set; }

        public int ClientId { get; set; }

        public virtual ClientDTO? Client { get; set; }

        public string? AdminId { get; set; }
        public UserDTO? Admin { get; set; }
    }
}
