using Domain.Entities.Old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class WaterEntryDTO
    {
        public WaterEntryDTO() { }
        public WaterEntryDTO(WaterEntry w)
        {
            Id = w.Id;
            Date = w.Date;
            Amount = w.Amount;
            UserId = w.UserId;
        }

        public WaterEntryDTO(WaterEntryDTO w)
        {
            Id = w.Id;
            Date = w.Date;
            Amount = w.Amount;
            UserId = w.UserId;
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string UserId { get; set; }
    }

}
