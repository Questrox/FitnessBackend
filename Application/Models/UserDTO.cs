using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserDTO
    {
        public UserDTO() { }
        public UserDTO(User u)
        {
            Id = u.Id;
            FullName = u.FullName;
            MealPlanStart = u.MealPlanStart;
            MealPlanId = u.MealPlanId;
            MealPlan = u.MealPlan != null ? new MealPlanDTO(u.MealPlan) : null;
            Food = u.Food.Select(f => new FoodDTO(f)).ToList();
            FoodEntry = u.FoodEntry.Select(fe => new FoodEntryDTO(fe)).ToList();
            WaterEntry = u.WaterEntry.Select(we => new WaterEntryDTO(we)).ToList();
        }

        public string Id { get; set; }
        public string FullName { get; set; }
        public DateTime MealPlanStart { get; set; }
        public int? MealPlanId { get; set; }
        public MealPlanDTO? MealPlan { get; set; }
        public ICollection<FoodDTO>? Food { get; set; }
        public ICollection<FoodEntryDTO>? FoodEntry { get; set; }
        public ICollection<WaterEntryDTO>? WaterEntry { get; set; }
    }
}