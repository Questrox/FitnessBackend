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
        public UserDTO() { }

        public UserDTO(User u)
        {
            Id = u.Id;
            Email = u.Email;
            UserName = u.UserName;
            PhoneNumber = u.PhoneNumber;
            FullName = u.FullName;
        }

        public UserDTO(UserDTO u)
        {
            Id = u.Id;
            Email = u.Email;
            UserName = u.UserName;
            PhoneNumber = u.PhoneNumber;
            FullName = u.FullName;
        }

        public string Id { get; set; } = null!;

        public string? Email { get; set; }

        public string? UserName { get; set; }

        public string? PhoneNumber { get; set; }

        public string FullName { get; set; } = null!;
    }
}
