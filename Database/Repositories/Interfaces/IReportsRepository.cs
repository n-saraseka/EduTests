using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface IReportsRepository : IRepository<Report, int>
{
    // For now, this repository only exists to keep things the same
    // as other repositories that have special queries.
}