using Domain.Entities.Old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Models.Old
{
    public class MealPlanDTO
    {
        public MealPlanDTO() { }
        public MealPlanDTO(MealPlan mp)
        {
            Id = mp.Id;
            Title = mp.Title;
            Description = mp.Description;
            FullDescription = mp.FullDescription;
            Benefits = JsonSerializer.Deserialize<List<string>>(mp.BenefitsJson);
            Warnings = mp.WarningsJson != null ? JsonSerializer.Deserialize<List<string>>(mp.WarningsJson) : null;
            Calories = mp.Calories;
            Protein = mp.Protein;
            Fat = mp.Fat;
            Carbs = mp.Carbs;
            MealPlanDay = mp.MealPlanDay.Select(d => new MealPlanDayDTO(d)).ToList();
        }

        public MealPlanDTO(MealPlanDTO mp)
        {
            Id = mp.Id;
            Title = mp.Title;
            Description = mp.Description;
            FullDescription = mp.FullDescription;
            Benefits = mp.Benefits;
            Warnings = mp.Warnings;
            Calories = mp.Calories;
            Protein = mp.Protein;
            Fat = mp.Fat;
            Carbs = mp.Carbs;
            MealPlanDay = mp.MealPlanDay;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FullDescription { get; set; }
        public List<string> Benefits { get; set; }
        public List<string>? Warnings { get; set; }
        public string Calories { get; set; }
        public string Protein { get; set; }
        public string Fat { get; set; }
        public string Carbs { get; set; }
        public ICollection<MealPlanDayDTO>? MealPlanDay { get; set; }
    }
}
