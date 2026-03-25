using System.ComponentModel;

namespace EduTests.Models;

public class LoginViewModel
{
    [DisplayName("Логин")]
    public required string Login { get; set; }
    [DisplayName("Пароль")]
    public required string Password { get; set; }
}