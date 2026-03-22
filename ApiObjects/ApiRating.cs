using System.ComponentModel.DataAnnotations;

namespace EduTests.ApiObjects;

public class ApiRating
{
    public required int TestId { get; set; }
    public required int UserId { get; set; }
    public required bool IsPositive { get; set; }
}