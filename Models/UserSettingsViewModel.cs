using EduTests.ApiObjects;

namespace EduTests.Models;

public class UserSettingsViewModel
{
    public required ApiUser User { get; set; }
    public List<ApiReport>? Reports { get; set; }
    public int? ReportPages { get; set; }
    public List<ApiBan>? Bans { get; set; }
    public int? BanPages { get; set; }
    public List<ApiTest>? Tests { get; set; }
    public int? TestPages { get; set; }
    public int? RowsPerTablePage { get; set; }
}