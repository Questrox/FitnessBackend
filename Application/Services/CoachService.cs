using Application.Models;
using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CoachService(FitnessDb _db, UserManager<User> _userManager, AuthService _authService, 
        ICoachRepository _coachRep, IWebHostEnvironment _env)
    {
        public async Task<IEnumerable<CoachDTO>> GetCoachesAsync()
        {
            var coaches = await _coachRep.GetCoachesAsync();
            return coaches.Select(c => new CoachDTO(c));
        }

        public async Task<IEnumerable<CoachDTO>> GetAvailableCoachesAsync(DateTime start, DateTime end)
        {
            var coaches = await _coachRep.GetAvailableCoachesAsync(start, end);
            return coaches.Select(c => new CoachDTO(c));
        }

        public async Task<CoachDTO?> GetCoachByIdAsync(int id)
        {
            var coach = await _coachRep.GetCoachByIdAsync(id);
            return coach == null ? null : new CoachDTO(coach);
        }

        public async Task<LoginCredentials> AddCoachAsync(CreateCoachDTO coach)
        {
            if (coach.Experience < 0)
                throw new ArgumentException($"Стаж не может быть отрицательным. Введенный стаж: {coach.Experience}");
            if (string.IsNullOrEmpty(coach.PhoneNumber))
                throw new ArgumentException("Необходимо ввести номер телефона");
            if (string.IsNullOrEmpty(coach.FullName))
                throw new ArgumentException("Необходимо ввести ФИО");

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var userName = await _authService.GenerateUsernameAsync();
                var password = _authService.GeneratePassword();
                RegisterModel model = new RegisterModel
                {
                    FullName = coach.FullName,
                    PhoneNumber = coach.PhoneNumber,
                    UserName = userName,
                    Password = password
                };
                var result = await _authService.RegisterAsync(model);
                if (result == null)
                    throw new ArgumentException("Не удалось создать пользователя");

                var newCoach = new Coach
                {
                    Experience = coach.Experience,
                    UserId = result
                };

                // Добавление фото
                if (coach.Image != null && coach.Image.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "Coaches");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(coach.Image.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await coach.Image.CopyToAsync(stream);
                    }

                    newCoach.PhotoPath = $"images/Coaches/{uniqueFileName}";
                }
                else
                    newCoach.PhotoPath = "";

                await _coachRep.AddAsync(newCoach);
                newCoach = await _coachRep.GetCoachByIdAsync(newCoach.Id);

                await transaction.CommitAsync();

                return new LoginCredentials { UserName = userName, Password = password };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<CoachDTO> UpdateCoachAsync(UpdateCoachDTO coach)
        {
            var existingCoach = await _coachRep.GetCoachByIdAsync(coach.Id) ??
                throw new KeyNotFoundException($"Тренер с Id {coach.Id} не найден");

            if (coach.Experience < 0)
                throw new ArgumentException($"Стаж не может быть отрицательным. Введенный стаж: {coach.Experience}");
            if (string.IsNullOrEmpty(coach.FullName))
                throw new ArgumentException("Необходимо ввести ФИО");
            if (string.IsNullOrEmpty(coach.PhoneNumber))
                throw new ArgumentException("Необходимо ввести номер телефона");

            string? newFileName = null;
            string? newFilePath = null;
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var user = existingCoach.User;

                user.FullName = coach.FullName;
                user.PhoneNumber = coach.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new ArgumentException(errors);
                }

                existingCoach.Experience = coach.Experience;

                string? oldPhotoPath = existingCoach.PhotoPath;
                if (coach.Image != null && coach.Image.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "Coaches");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    newFileName = $"{Guid.NewGuid()}{Path.GetExtension(coach.Image.FileName)}";
                    newFilePath = Path.Combine(uploadsFolder, newFileName);

                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await coach.Image.CopyToAsync(stream);
                    }
                    existingCoach.PhotoPath = $"images/Coaches/{newFileName}";
                }

                await _coachRep.UpdateAsync(existingCoach);
                await transaction.CommitAsync();

                // Если транзакция прошла, удаляем старый файл
                if (newFileName != null && !string.IsNullOrEmpty(oldPhotoPath))
                {
                    var oldFullPath = Path.Combine(_env.WebRootPath, oldPhotoPath);
                    if (File.Exists(oldFullPath))
                        File.Delete(oldFullPath);
                }

                return new CoachDTO(existingCoach);
            }
            catch
            {
                await transaction.RollbackAsync();
                // Удаляем новый файл, если транзакция не прошла
                if (newFilePath != null && File.Exists(newFilePath))
                    File.Delete(newFilePath);
                throw;
            }
        }

        public async Task DeleteCoach(int coachId)
        {
            var coach = await _coachRep.GetCoachByIdAsync(coachId) ??
                throw new KeyNotFoundException($"Тренер с Id {coachId} не найден");

            await _coachRep.DeleteAsync(coach);
        }

        public async Task SoftDeleteCoach(int coachId)
        {
            var coach = await _coachRep.GetCoachByIdAsync(coachId) ??
                throw new KeyNotFoundException($"Тренер с Id {coachId} не найден");

            await _coachRep.SoftDeleteAsync(coach);
        }
    }
}
