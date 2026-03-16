using Domain.Entities.Old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Old
{
    public class FoodEntryDTO
    {
        public FoodEntryDTO() { }
        public FoodEntryDTO(FoodEntry fe)
        {
            Id = fe.Id;
            Date = fe.Date;
            Weight = fe.Weight;
            FoodId = fe.FoodId;
            MealTypeId = fe.MealTypeId;
            UserId = fe.UserId;
            Food = fe.Food != null ? new FoodDTO(fe.Food) : null;
            MealType = fe.MealType;
        }

        public FoodEntryDTO(FoodEntryDTO fe)
        {
            Id = fe.Id;
            Date = fe.Date;
            Weight = fe.Weight;
            FoodId = fe.FoodId;
            MealTypeId = fe.MealTypeId;
            UserId = fe.UserId;
            Food = fe.Food;
            MealType = fe.MealType;
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Weight { get; set; }
        public int FoodId { get; set; }
        public int MealTypeId { get; set; }
        public string UserId { get; set; }
        public virtual FoodDTO? Food { get; set; }
        public virtual MealType? MealType { get; set; }
    }
}
