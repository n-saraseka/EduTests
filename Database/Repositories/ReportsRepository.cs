using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;

namespace EduTests.Database.Repositories;

public class ReportsRepository(DatabaseContext db) : BaseRepository<Report, int>(db), IReportsRepository
{
    // For now, this repository only exists to keep things the same
    // as other repositories that have special queries.
}