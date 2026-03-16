using Domain.Entities.Old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Old
{
    public class MealPlanDayDTO
    {
        public MealPlanDayDTO() { }
        public MealPlanDayDTO(MealPlanDay d)
        {
            Id = d.Id;
            Day = d.Day;
            Breakfast = d.Breakfast;
            Lunch = d.Lunch;
            Dinner = d.Dinner;
            Snacks = d.Snacks;
            MealPlanId = d.MealPlanId;
        }

        public MealPlanDayDTO(MealPlanDayDTO d)
        {
            Id = d.Id;
            Day = d.Day;
            Breakfast = d.Breakfast;
            Lunch = d.Lunch;
            Dinner = d.Dinner;
            Snacks = d.Snacks;
            MealPlanId = d.MealPlanId;
        }

        public int Id { get; set; }
        public int Day { get; set; }
        public string Breakfast { get; set; }
        public string Lunch { get; set; }
        public string Dinner { get; set; }
        public string? Snacks { get; set; }
        public int MealPlanId { get; set; }
    }
}
