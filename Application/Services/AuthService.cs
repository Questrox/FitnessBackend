using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;
using Application.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Application.Models.DTOs;

namespace Application.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string?> RegisterAsync(RegisterModel model)
        {
            var user = new User
            {
                UserName = model.UserName,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber
            };

            _logger.LogInformation($"Попытка регистрации пользователя {model.UserName}");

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    _logger.LogError($"Ошибка регистрации: {error.Code} {error.Description}");
                return null;
            }

            await _userManager.AddToRoleAsync(user, "User");

            _logger.LogInformation($"Пользователь {model.UserName} успешно зарегистрирован");
            return user.Id;
        }

        public async Task<LoginResult?> LoginAsync(LoginModel model)
        {
            _logger.LogInformation($"Попытка входа пользователя {model.UserName}");

            var result = await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                isPersistent: true,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                _logger.LogError($"Пользователю {model.UserName} не удалось выполнить вход");
                return null;
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            var roles = await _userManager.GetRolesAsync(user);

            var token = GenerateJwtToken(user, roles);

            _logger.LogInformation($"Пользователь {model.UserName} успешно вошел в систему");

            return new LoginResult
            {
                Token = token,
                UserName = user.UserName,
                UserRole = roles.FirstOrDefault()
            };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<ValidateResult?> ValidateTokenAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user == null)
            {
                _logger.LogWarning("Пользователь не аутентифицирован");
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new ValidateResult
            {
                Message = "Сессия активна",
                UserName = user.UserName,
                UserRole = roles.FirstOrDefault()
            };
        }

        private string GenerateJwtToken(User user, IList<string> roles)
        {
            _logger.LogInformation($"Генерируется токен для пользователя {user.UserName}");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            claims.AddRange(roles.Select(role =>
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddDays(
                Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateUsernameAsync(int length = 6)
        {
            while (true)
            {
                const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";

                var result = new StringBuilder();

                for (int i = 0; i < length; i++)
                {
                    result.Append(chars[RandomNumberGenerator.GetInt32(chars.Length)]);
                }
                if (await _userManager.FindByNameAsync(result.ToString()) == null)
                    return result.ToString();
            }
        }

        public string GeneratePassword(int length = 8)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string special = "!@$?_-";

            var all = lower + upper + digits + special;

            var chars = new List<char>();

            // гарантируем наличие каждого типа
            chars.Add(upper[RandomNumberGenerator.GetInt32(upper.Length)]);
            chars.Add(lower[RandomNumberGenerator.GetInt32(lower.Length)]);
            chars.Add(digits[RandomNumberGenerator.GetInt32(digits.Length)]);
            chars.Add(special[RandomNumberGenerator.GetInt32(special.Length)]);

            for (int i = 4; i < length; i++)
            {
                chars.Add(all[RandomNumberGenerator.GetInt32(all.Length)]);
            }

            // перемешивание
            for (int i = chars.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }

            return new string(chars.ToArray());
        }

        public async Task<UserDTO> UpdateUserAsync(UserDTO updated)
        {
            var user = await _userManager.FindByIdAsync(updated.Id);
            if (user == null)
                throw new ArgumentException("Не удалось найти пользователя с данным Id");

            if (string.IsNullOrWhiteSpace(updated.UserName))
                throw new ArgumentException("Логин не может быть пустым");
            if (string.IsNullOrWhiteSpace(updated.FullName))
                throw new ArgumentException("ФИО не может быть пустым");
            if (string.IsNullOrWhiteSpace(updated.PhoneNumber))
                throw new ArgumentException("Номер телефона не может быть пустым");

            var existingUser = await _userManager.FindByNameAsync(updated.UserName);
            if (existingUser != null && existingUser.Id != user.Id)
                throw new ArgumentException($"Логин {updated.UserName} уже используется");

            user.FullName = updated.FullName;
            user.UserName = updated.UserName;
            user.PhoneNumber = updated.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ArgumentException(string.Join("\n", result.Errors.Select(e => e.Description)));

            return new UserDTO(user);
        }

        public async Task ResetPassword(string userId, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ArgumentException($"Не удалось найти пользователя с данным Id");

            _logger.LogInformation($"Пользователь {user.UserName} меняет пароль");
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!result.Succeeded)
            {
                string message = "";
                foreach (var error in result.Errors)
                    message += error.Description + "\n";

                _logger.LogWarning($"Ошибка смены пароля для пользователя {user.UserName}: {message}");
                throw new InvalidOperationException(message);
            }
        }

        public async Task<LoginCredentials> GenerateNewCredentials(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new ArgumentException($"Не удалось найти пользователя с данным Id");

            string userName = await GenerateUsernameAsync();
            string password = GeneratePassword();
            user.UserName = userName;
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (!result.Succeeded)
            {
                string message = "";
                foreach (var error in result.Errors)
                    message += error.Description + "\n";

                _logger.LogWarning($"Ошибка генерации данных для входа для пользователя {user.UserName}: {message}");
                throw new InvalidOperationException(message);
            }
            LoginCredentials credentials = new LoginCredentials { UserName = userName, Password = password };
            return credentials;
        }
    }
}
