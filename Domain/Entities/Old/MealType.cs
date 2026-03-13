using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities.Old
{
    public class MealType
    {
        public MealType()
        {
            FoodEntry = new HashSet<FoodEntry>();
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<FoodEntry> FoodEntry { get; set; }
    }
}
