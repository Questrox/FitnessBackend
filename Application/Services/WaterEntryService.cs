using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class WaterEntryService
{
    private readonly IWaterEntryRepository _WERepository;
    public WaterEntryService(IWaterEntryRepository WERepository)
    {
        _WERepository = WERepository;
    }
    public async Task<IEnumerable<WaterEntryDTO>> GetUserWaterEntriesByDateAsync(string userId, DateTime date)
    {
        var entries = await _WERepository.GetUserWaterEntriesByDateAsync(userId, date);
        return entries.Select(e => new WaterEntryDTO(e));
    }
    public async Task<WaterEntryDTO> AddWaterEntryAsync(CreateWaterEntryDTO entry)
    {
        var newWE = new WaterEntry
        {
            Date = entry.Date,
            Amount = entry.Amount,
            UserId = entry.UserId,
        };
        await _WERepository.AddWaterEntryAsync(newWE);
        return new WaterEntryDTO(newWE);
    }
    public async Task DeleteFoodEntryAsync(int id)
    {
        await _WERepository.DeleteWaterEntryAsync(id);
    }
}
