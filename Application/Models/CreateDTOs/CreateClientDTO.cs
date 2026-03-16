using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.CreateDTOs
{
    public class CreateClientDTO
    {
        public decimal Bonuses { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string ConfirmationCode { get; set; } = null!;
        public string? UserId { get; set; }
    }
}
