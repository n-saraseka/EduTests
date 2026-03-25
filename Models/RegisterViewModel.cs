using System.ComponentModel;

namespace EduTests.Models;

public class RegisterViewModel
{
    [DisplayName("Логин")]
    public required string Login { get; set; }
    [DisplayName("Пароль")]
    public required string Password { get; set; }
    [DisplayName("Подтвердите пароль")]
    public required string ConfirmPassword { get; set; }
    [DisplayName("Имя пользователя")]
    public required string Username { get; set; }
}