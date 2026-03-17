using EduTests.Database.Entities;
using EduTests.Database.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class UserRepository(DatabaseContext db) : BaseRepository<User, int>(db)
{
    public Task<List<User>> GetByGroupAsync(UserGroup group, CancellationToken cancellationToken = default) =>
        Set.Where(u => u.Group == group).ToListAsync(cancellationToken);
}