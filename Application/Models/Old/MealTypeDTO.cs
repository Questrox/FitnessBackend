using Domain.Entities.Old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Old
{
    public class MealTypeDTO
    {
        public MealTypeDTO() { }
        public MealTypeDTO(MealType m)
        {
            Id = m.Id;
            Name = m.Name;
            FoodEntry = m.FoodEntry.Select(fe => new FoodEntryDTO(fe)).ToList();
        }

        public MealTypeDTO(MealTypeDTO m)
        {
            Id = m.Id;
            Name = m.Name;
            FoodEntry = m.FoodEntry;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<FoodEntryDTO>? FoodEntry { get; set; }
    }
}
