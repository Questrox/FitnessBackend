using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.CreateDTOs
{
    public class CreateTrainingTypeDTO
    {
        public decimal Price { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int CashbackPercentage { get; set; }
    }
}
