using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EduTests.Models;

public class RegisterViewModel
{
    [DisplayName("Логин")]
    public required string Login { get; set; }
    [DisplayName("Пароль")]
    public required string Password { get; set; }
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DisplayName("Подтвердите пароль")]
    public required string ConfirmPassword { get; set; }
    [DisplayName("Имя пользователя")]
    public required string Username { get; set; }
}