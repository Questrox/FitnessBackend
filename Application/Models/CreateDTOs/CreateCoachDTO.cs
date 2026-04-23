using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.CreateDTOs
{
    public class CreateCoachDTO
    {
        public int Experience { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile Image { get; set; } = null!;
    }
}
