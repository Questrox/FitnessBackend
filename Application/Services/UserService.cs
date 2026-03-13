using Application.DTOs;
using Domain.Interfaces;

namespace Application.Services;

/// <summary>
/// Сервис для управления пользователями
/// </summary>
public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    /// <summary>
    /// Получает пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Пользователь или null, если не найден</returns>
    public async Task<UserDTO> GetUserByIdAsync(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
            return null;
        return new UserDTO(user);
    }
    /// <summary>
    /// Обновляет данные пользователя
    /// </summary>
    /// <param name="userDTO">Обновляемый пользователь</param>
    /// <returns>Обновленный пользователь или null, если не найден</returns>
    public async Task<UserDTO?> UpdateUserAsync(UserDTO userDTO)
    {
        var existingUser = await _userRepository.GetUserByIdAsync(userDTO.Id);

        existingUser.FullName = userDTO.FullName;
        existingUser.MealPlanId = userDTO.MealPlanId;
        existingUser.MealPlanStart = userDTO.MealPlanStart;

        await _userRepository.UpdateUserAsync(existingUser);

        return new UserDTO(existingUser); // Возвращаем обновленный объект
    }
}
