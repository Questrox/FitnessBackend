using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    #region Классы для чтения json
    public class FoodInfo
    {
        public int calories_per_100g { get; set; }
        public double protein_per_100g { get; set; }
        public double carbs_per_100g { get; set; }
        public double fat_per_100g { get; set; }
        public double fiber_per_100g { get; set; }
        public string name { get; set; }
    }
    public class MealPlanJson
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string fullDescription { get; set; }
        public List<string> benefits { get; set; }
        public List<string>? warnings { get; set; }
        public Macros macros { get; set; }
        public List<DayMenuJson> weekMenu { get; set; }
    }

    public class Macros
    {
        public string calories { get; set; }
        public string protein { get; set; }
        public string fat { get; set; }
        public string carbs { get; set; }
    }

    public class DayMenuJson
    {
        public int day { get; set; }
        public string breakfast { get; set; }
        public string lunch { get; set; }
        public string dinner { get; set; }
        public string? snacks { get; set; }
    }
    #endregion

    public static class DbInitializer
    {
        public static async Task Initialize(CaloriesDb context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            #region Пользователи
            // Проверяем наличие ролей
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            // Проверяем наличие пользователей
            if (!userManager.Users.Any())
            {
                var user = new User
                {
                    UserName = "user",
                    Email = "user@example.com",
                    FullName = "Иванов Иван Иванович",
                };
                await userManager.CreateAsync(user, "User@123");
                await userManager.AddToRoleAsync(user, "user");
            }
            #endregion

            #region Еда
            if (!context.Foods.Any())
            {
                var foodJson = File.ReadAllText("nutritional_database_translated.json");
                var foods = JsonSerializer.Deserialize<Dictionary<string, FoodInfo>>(foodJson);

                foreach (var item in foods)
                {
                    string english = item.Key;
                    var info = item.Value;

                    context.Foods.Add(new Food
                    {
                        Name = info.name,
                        EngName = english,
                        Calories = info.calories_per_100g,
                        Protein = info.protein_per_100g,
                        Fat = info.fat_per_100g,
                        Carbs = info.carbs_per_100g,
                        UserId = null // админский продукт
                    }); ;
                }

                await context.SaveChangesAsync();
            }
            #endregion

            #region Планы питания

            if (!context.MealPlans.Any())
            {
                var plansJson = File.ReadAllText("meal-plans-data.json");
                var plans = JsonSerializer.Deserialize<List<MealPlanJson>>(plansJson);

                foreach (var p in plans)
                {
                    var entity = new MealPlan
                    {
                        Title = p.title,
                        Description = p.description,
                        FullDescription = p.fullDescription,
                        BenefitsJson = JsonSerializer.Serialize(p.benefits),
                        WarningsJson = p.warnings != null ? JsonSerializer.Serialize(p.warnings) : null,
                        Calories = p.macros.calories,
                        Protein = p.macros.protein,
                        Fat = p.macros.fat,
                        Carbs = p.macros.carbs
                    };

                    context.MealPlans.Add(entity);
                    await context.SaveChangesAsync();

                    foreach (var day in p.weekMenu)
                    {
                        context.MealPlanDays.Add(new MealPlanDay
                        {
                            MealPlanId = entity.Id,
                            Day = day.day,
                            Breakfast = day.breakfast,
                            Lunch = day.lunch,
                            Dinner = day.dinner,
                            Snacks = day.snacks
                        });
                    }
                }

                await context.SaveChangesAsync();
            }
            #endregion

        }
    }
}