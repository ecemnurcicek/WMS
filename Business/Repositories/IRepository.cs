using Business.Exceptions;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Repositories;

/// <summary>
/// Generic Repository Pattern - Tüm veri erişim işlemlerinde kullanılır
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
