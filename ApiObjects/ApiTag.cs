using System.ComponentModel.DataAnnotations;

namespace EduTests.ApiObjects;

public class ApiTag
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
}