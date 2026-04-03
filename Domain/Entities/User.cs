using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class User : IdentityUser, ISoftDeletable
    {
        public User() { }
        public string FullName { get; set; } = null!;
        [JsonIgnore]
        public virtual Client? Client { get; set; }
        [JsonIgnore]
        public virtual Coach? Coach { get; set; }
        [JsonIgnore]
        public virtual ICollection<Payment> Payments { get; set; }
        [JsonIgnore]
        public virtual ICollection<CancellationNotification> CancellationNotifications { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
