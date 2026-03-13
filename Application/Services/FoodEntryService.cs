using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class FoodEntryService
{
    private readonly IFoodEntryRepository _FERepository;
    public FoodEntryService(IFoodEntryRepository fERepository)
    {
        _FERepository = fERepository;
    }
    public async Task<IEnumerable<FoodEntryDTO>> GetUserFoodEntriesByDateAsync(string userId, DateTime date)
    {
        var entries = await _FERepository.GetUserFoodEntriesByDateAsync(userId, date);
        return entries.Select(e => new FoodEntryDTO(e));
    }
    public async Task<FoodEntryDTO> AddFoodEntryAsync(CreateFoodEntryDTO entry)
    {
        var newFE = new FoodEntry
        {
            Date = entry.Date,
            Weight = entry.Weight,
            FoodId = entry.FoodId,
            MealTypeId = entry.MealTypeId,
            UserId = entry.UserId
        };
        await _FERepository.AddFoodEntryAsync(newFE);
        newFE = await _FERepository.GetFoodEntryByIdAsync(newFE.Id);
        return new FoodEntryDTO(newFE);
    }
    public async Task DeleteFoodEntryAsync(int id)
    {
        await _FERepository.DeleteFoodEntryAsync(id);
    }
}
