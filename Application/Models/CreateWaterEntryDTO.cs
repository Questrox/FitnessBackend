using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateWaterEntryDTO
    {
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string? UserId { get; set; }
    }
}
