namespace EduTests.Models;

public class RegisterViewModel
{
    public required string Login { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public required string Username { get; set; }
    public string? ReturnUrl { get; set; }
}