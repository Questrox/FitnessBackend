using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateFoodEntryDTO
    {
        public DateTime Date { get; set; }
        public double Weight { get; set; }
        public int FoodId { get; set; }
        public int MealTypeId { get; set; }
        public string? UserId { get; set; }
    }
}
