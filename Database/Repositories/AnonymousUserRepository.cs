using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;

namespace EduTests.Database.Repositories;

public class AnonymousUserRepository(DatabaseContext db) : BaseRepository<AnonymousUser, Guid>(db), IAnonymousUserRepository
{
    // Only exists to keep things the same as other used repositories for now.
}