using Application.DTOs;
using Domain.Interfaces;

namespace Application.Services;

public class MealPlanService
{
    private readonly IMealPlanRepository _MPRepository;
    public MealPlanService(IMealPlanRepository MPRepository)
    {
        _MPRepository = MPRepository;
    }
    public async Task<IEnumerable<MealPlanDTO>> GetMealPlansAsync()
    {
        var plans = await _MPRepository.GetMealPlansAsync();
        return plans.Select(p => new MealPlanDTO(p));
    }
    public async Task<MealPlanDTO> GetMealPlanAsync(int id)
    {
        var plan = await _MPRepository.GetMealPlanAsync(id);
        return new MealPlanDTO(plan);
    }
}
