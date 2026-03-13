using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateMealPlanDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string FullDescription { get; set; }
        public string BenefitsJson { get; set; } = "[]";
        public string? WarningsJson { get; set; }
        public string Calories { get; set; }
        public string Protein { get; set; }
        public string Fat { get; set; }
        public string Carbs { get; set; }
    }
}
