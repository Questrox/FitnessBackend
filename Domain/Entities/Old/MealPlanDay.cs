using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities.Old
{
    public class MealPlanDay
    {
        [Key]
        public int Id { get; set; }
        public int Day { get; set; }
        public string Breakfast { get; set; } = null!;
        public string Lunch { get; set; } = null!;
        public string Dinner { get; set; } = null!;
        public string? Snacks { get; set; }

        public int MealPlanId { get; set; }
        public MealPlan MealPlan { get; set; } = null!;
    }

}
