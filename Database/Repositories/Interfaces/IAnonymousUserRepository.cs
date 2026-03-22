using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface IAnonymousUserRepository : IRepository<AnonymousUser, Guid>
{
    // Only exists to keep things the same as other used repositories for now.
}