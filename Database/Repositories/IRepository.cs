using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public interface IRepository<T>
    where T : class
{
    DbSet<T> GetAll();
    ValueTask<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<T>> GetBulkAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    void Create(T item);
    void CreateBulk(IEnumerable<T> items);
    Task UpdateAsync(T item, CancellationToken cancellationToken = default);
    Task UpdateBulkAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);
    Task DeleteAsync(T item, CancellationToken cancellationToken = default);
    Task DeleteBulkAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}