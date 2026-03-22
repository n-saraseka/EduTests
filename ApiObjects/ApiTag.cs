using System.ComponentModel.DataAnnotations;

namespace EduTests.ApiObjects;

public class ApiTag
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}