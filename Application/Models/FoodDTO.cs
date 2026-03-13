using Domain.Entities.Old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class FoodDTO
    {
        public FoodDTO() { }
        public FoodDTO(Food f)
        {
            Id = f.Id;
            Name = f.Name;
            EngName = f.EngName;
            Calories = f.Calories;
            Protein = f.Protein;
            Fat = f.Fat;
            Carbs = f.Carbs;
        }

        public FoodDTO(FoodDTO f)
        {
            Id = f.Id;
            Name = f.Name;
            EngName = f.EngName;
            Calories = f.Calories;
            Protein = f.Protein;
            Fat = f.Fat;
            Carbs = f.Carbs;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string? EngName { get; set; }
        public int Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }
    }

}
