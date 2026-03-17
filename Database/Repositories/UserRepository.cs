using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class UserRepository(DatabaseContext db) : IRepository<User>
{
    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>DbSet with all user data</returns>
    public DbSet<User> GetAll()
    {
        var all = db.Users;
        return all;
    }
    
    /// <summary>
    /// Get a user by their ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">A CancellationToken to observe</param>
    /// <returns>User object or null</returns>
    /// <exception cref="OperationCanceledException">If the CancellationToken is canceled</exception>
    public ValueTask<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return db.Users.FindAsync([id], cancellationToken);
    }
    
    /// <summary>
    /// Get users by their IDs
    /// </summary>
    /// <param name="ids">User IDs</param>
    /// <param name="cancellationToken">A CancellationToken to observe</param>
    /// <returns>List containing existing users with such IDs</returns>
    /// <exception cref="OperationCanceledException">If the CancellationToken is canceled</exception>
    public Task<List<User>> GetBulkAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return db.Users
            .Where(u => ids.Contains(u.Id)) 
            .ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// Add user
    /// </summary>
    /// <param name="item">Populated User object</param>
    public void Create(User item)
    {
        db.Users.Add(item);
    }
    
    /// <summary>
    /// Add users
    /// </summary>
    /// <param name="items">IEnumerable containing populated User objects</param>
    public void CreateBulk(IEnumerable<User> items)
    {
        db.Users.AddRange(items);
    }
    
    /// <summary>
    /// Update user data
    /// </summary>
    /// <param name="item">Populated User object</param>
    /// <param name="cancellationToken">A CancellationToken to observe</param>
    /// <exception cref="OperationCanceledException">If the CancellationToken is canceled</exception>
    public async Task UpdateAsync(User item, CancellationToken cancellationToken = default)
    {
        var old = await GetByIdAsync(item.Id, cancellationToken);
        if (old != null)
            db.Users.Update(item);
    }
    
    /// <summary>
    /// Update users data
    /// </summary>
    /// <param name="items">IEnumerable containing populated User objects</param>
    /// <param name="cancellationToken">A CancellationToken to observe</param>
    /// <exception cref="OperationCanceledException">If the CancellationToken is canceled</exception>
    public async Task UpdateBulkAsync(IEnumerable<User> items, CancellationToken cancellationToken = default)
    {
        var old = await GetBulkAsync(items.Select(u => u.Id), cancellationToken);
        var toUpdate = items.Where(u => old.Select(o => o.Id).Contains(u.Id));
        if (old.Count > 0)
            db.Users.UpdateRange(toUpdate);
    }
    
    /// <summary>
    /// Delete user data
    /// </summary>
    /// <param name="item">Populated User object</param>
    /// <param name="cancellationToken">A CancellationToken to observe</param>
    /// <exception cref="OperationCanceledException">If the CancellationToken is canceled</exception>
    public async Task DeleteAsync(User item, CancellationToken cancellationToken = default)
    {
        var toDelete = await GetByIdAsync(item.Id, cancellationToken);
        if (toDelete != null)
            db.Users.Remove(toDelete);
    }
    
    /// <summary>
    /// Delete users data
    /// </summary>
    /// <param name="items">IEnumerable containing populated User objects</param>
    /// <param name="cancellationToken">A CancellationToken to observe</param>
    /// <exception cref="OperationCanceledException">If the CancellationToken is canceled</exception>
    public async Task DeleteBulkAsync(IEnumerable<User> items, CancellationToken cancellationToken = default)
    {
        var toDelete = await GetBulkAsync(items.Select(u => u.Id), cancellationToken);
        if (toDelete.Count > 0)
            db.Users.RemoveRange(toDelete);
    }
    
    /// <summary>
    /// Save changes to the users table
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken to observe</param>
    /// <exception cref="OperationCanceledException">If the CancellationToken is canceled</exception>
    /// <returns>Task that represents the asynchronous save operation. Task result contains the number of state entries written to the DB</returns>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return db.SaveChangesAsync(cancellationToken);
    }
}