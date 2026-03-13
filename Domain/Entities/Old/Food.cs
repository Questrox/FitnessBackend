using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities.Old
{
    public class Food
    {
        public Food()
        {
            FoodEntry = new HashSet<FoodEntry>();
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? EngName { get; set; }
        public int Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }
        public string? UserId { get; set; }
        public virtual User? User { get; set; }
        [JsonIgnore]
        public virtual ICollection<FoodEntry> FoodEntry { get; set; }
    }
}
