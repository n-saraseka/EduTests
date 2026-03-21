using System.ComponentModel.DataAnnotations;

namespace EduTests.ApiObjects;

public class ApiRating
{
    [Required]
    public int TestId { get; set; }
    [Required]
    public int UserId { get; set; }
    [Required]
    public bool IsPositive { get; set; }
}