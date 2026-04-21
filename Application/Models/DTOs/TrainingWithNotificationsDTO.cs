using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class TrainingWithNotificationsDTO
    {
        public TrainingWithNotificationsDTO(Training t)
        {
            Training = new TrainingDTO(t);
            Notifications = t.CancellationNotifications.Select(c => new CancellationNotificationDTO(c)).ToList();
            NotNotifiedCount = t.CancellationNotifications.Count(c => !c.IsNotified);
        }
        public TrainingDTO Training { get; set; } = null!;
        public List<CancellationNotificationDTO> Notifications { get; set; } = new();
        public int NotNotifiedCount { get; set; }
    }
}
