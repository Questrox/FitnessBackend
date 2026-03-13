using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class FoodService
{
    private readonly IFoodRepository _FRepository;
    public FoodService(IFoodRepository fRepository)
    {
        _FRepository = fRepository;
    }
    public async Task<IEnumerable<FoodDTO>> GetAllFoodsForUserAsync(string userId)
    {
        var foods = await _FRepository.GetAllFoodsForUserAsync(userId);
        return foods.Select(f => new FoodDTO(f));
    }
    public async Task<FoodDTO> AddFoodAsync(CreateFoodDTO food)
    {
        var newFood = new Food
        {
            Name = food.Name,
            EngName = food.EngName,
            Calories = food.Calories,
            Protein = food.Protein,
            Fat = food.Fat,
            Carbs = food.Carbs,
            UserId = food.UserId
        };
        await _FRepository.AddFoodAsync(newFood);
        return new FoodDTO(newFood);
    }
}
