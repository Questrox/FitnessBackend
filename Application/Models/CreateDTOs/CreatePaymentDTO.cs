using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.CreateDTOs
{
    public class CreatePaymentDTO
    {
        public DateTime Date { get; set; }

        public decimal Price { get; set; }

        public int CashbackPercentage { get; set; }

        public decimal PaidWithBonuses { get; set; }
    }
}
