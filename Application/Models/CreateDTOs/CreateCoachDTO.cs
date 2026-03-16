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
        public string? PhotoPath { get; set; }
        public string? UserId { get; set; }
    }
}
