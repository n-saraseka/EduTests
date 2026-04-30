using System.ComponentModel.DataAnnotations;
using EduTests.Database.Enums;

namespace EduTests.Commands.UserCommands;

public class ChangeUserGroupCommand
{
    [Required]
    public UserGroup UserGroup { get; set; }
}