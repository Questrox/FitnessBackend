using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateUserDTO
    {
        public string FullName { get; set; }
        public DateTime MealPlanStart { get; set; }
        public int? MealPlanId { get; set; }
    }
}
