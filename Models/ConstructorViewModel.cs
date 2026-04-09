using EduTests.ApiObjects;

namespace EduTests.Models;

public class ConstructorViewModel
{
    public required ApiUser User { get; set; }
    public ApiTest? Test { get; set; }
}