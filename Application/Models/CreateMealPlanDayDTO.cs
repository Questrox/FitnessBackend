using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateMealPlanDayDTO
    {
        public int Day { get; set; }
        public string Breakfast { get; set; }
        public string Lunch { get; set; }
        public string Dinner { get; set; }
        public string? Snacks { get; set; }
        public int MealPlanId { get; set; }
    }
}
