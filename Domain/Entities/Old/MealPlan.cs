using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities.Old
{
    public class MealPlan
    {
        public MealPlan()
        {
            MealPlanDay = new HashSet<MealPlanDay>();
            User = new HashSet<User>();
        }
        [Key]
        public int Id { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string FullDescription { get; set; } = null!;

        public string BenefitsJson { get; set; } = "[]";
        public string? WarningsJson { get; set; }

        public string Calories { get; set; } = null!;
        public string Protein { get; set; } = null!;
        public string Fat { get; set; } = null!;
        public string Carbs { get; set; } = null!;
        [JsonIgnore]
        public ICollection<MealPlanDay> MealPlanDay { get; set; }
        [JsonIgnore]
        public ICollection<User> User { get; set; }
    }

}
