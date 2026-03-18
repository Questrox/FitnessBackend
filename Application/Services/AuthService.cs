using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;
using Application.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

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

        public async Task<bool> RegisterAsync(RegisterModel model)
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
                return false;
            }

            await _userManager.AddToRoleAsync(user, "User");

            _logger.LogInformation($"Пользователь {model.UserName} успешно зарегистрирован");
            return true;
        }

        public async Task<LoginResult?> LoginAsync(LoginModel model)
        {
            _logger.LogInformation($"Попытка входа пользователя {model.UserName}");

            var result = await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                isPersistent: false,
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
    }
}
