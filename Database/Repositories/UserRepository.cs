using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class UserRepository(DatabaseContext db) : BaseRepository<User, int>(db), IUserRepository
{
    /// <summary>
    /// Get all <see cref="User"/>s within a <see cref="UserGroup"/>
    /// </summary>
    /// <param name="group">A <see cref="UserGroup"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>List of all <see cref="User"/>s within a <see cref="UserGroup"/></returns>
    /// <exception cref="TaskCanceledException">If the <see cref="CancellationToken"/> is canceled</exception>
    public Task<List<User>> GetByGroupAsync(UserGroup group, CancellationToken cancellationToken = default) =>
        Set.Where(u => u.Group == group).ToListAsync(cancellationToken);
    
    public Task<User?> GetByLoginAsync(string login,CancellationToken cancellationToken = default) =>
        Set.Where(u => u.Login == login).FirstOrDefaultAsync(cancellationToken);
}