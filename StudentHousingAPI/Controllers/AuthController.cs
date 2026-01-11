using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentHousingAPI.Data;
using StudentHousingAPI.DTOs;
using StudentHousingAPI.Services;

namespace StudentHousingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;

    public AuthController(
        ApplicationDbContext context,
        ITokenService tokenService,
        IPasswordService passwordService)
    {
        _context = context;
        _tokenService = tokenService;
        _passwordService = passwordService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var token = _tokenService.GenerateToken(user);

        return Ok(new LoginResponse
        {
            Token = token,
            Role = user.Role,
            UserId = user.Id,
            Username = user.Username
        });
    }
}
