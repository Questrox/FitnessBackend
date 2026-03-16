using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class UserDTO
    {
        public UserDTO()
        {
            Clients = new HashSet<ClientDTO>();
            Coaches = new HashSet<CoachDTO>();
        }

        public UserDTO(User u)
        {
            Id = u.Id;
            Email = u.Email;
            UserName = u.UserName;
            PhoneNumber = u.PhoneNumber;
            FullName = u.FullName;

            Clients = u.Clients
                .Select(x => new ClientDTO(x))
                .ToList();

            Coaches = u.Coaches
                .Select(x => new CoachDTO(x))
                .ToList();
        }

        public UserDTO(UserDTO u)
        {
            Id = u.Id;
            Email = u.Email;
            UserName = u.UserName;
            PhoneNumber = u.PhoneNumber;
            FullName = u.FullName;
            Clients = u.Clients;
            Coaches = u.Coaches;
        }

        public string Id { get; set; } = null!;

        public string? Email { get; set; }

        public string? UserName { get; set; }

        public string? PhoneNumber { get; set; }

        public string FullName { get; set; } = null!;

        public virtual ICollection<ClientDTO> Clients { get; set; }

        public virtual ICollection<CoachDTO> Coaches { get; set; }
    }
}
