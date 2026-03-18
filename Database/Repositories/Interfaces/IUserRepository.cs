using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Database.Repositories.Interfaces;

public interface IUserRepository : IRepository<User, int>
{
    Task<List<User>> GetByGroupAsync(UserGroup group, CancellationToken cancellationToken = default);

    Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
}