using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using Domain.Entities.Old;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public User()
        {
            Clients = new HashSet<Client>();
            Coaches = new HashSet<Coach>();
        }
        public string FullName { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Client> Clients { get; set; }
        [JsonIgnore]
        public virtual ICollection<Coach> Coaches { get; set; }

    }
}
