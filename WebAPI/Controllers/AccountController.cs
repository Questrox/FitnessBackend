using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(
            AuthService authService,
            ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _authService.RegisterAsync(model);

            if (!success)
                return BadRequest("Ошибка регистрации");

            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResult>> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model);

            if (result == null)
                return Unauthorized();

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation($"Пользователь {User.Identity?.Name} выполняет выход");

            await _authService.LogoutAsync();

            return Ok(new { Message = "User logged out successfully" });
        }

        [HttpGet("validate")]
        public async Task<ActionResult<ValidateResult>> ValidateToken()
        {
            var result = await _authService.ValidateTokenAsync(User);

            if (result == null)
                return Unauthorized(new { message = "Вы Гость. Пожалуйста, выполните вход" });

            return Ok(result);
        }
    }
}