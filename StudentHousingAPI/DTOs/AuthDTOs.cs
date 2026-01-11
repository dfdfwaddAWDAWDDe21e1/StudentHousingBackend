namespace StudentHousingAPI.DTOs;

public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class LoginResponse
{
    public required string Token { get; set; }
    public required string Role { get; set; }
    public required int UserId { get; set; }
    public required string Username { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public int? BuildingId { get; set; }
}
