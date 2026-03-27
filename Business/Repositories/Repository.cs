using Business.Exceptions;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Repositories;

/// <summary>
/// Generic Repository Implementation
/// Tüm CRUD işlemlerinde otomatik hata yönetimi yapılır
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationContext _context;
    private readonly DbSet<T> _dbSet;
    private readonly string _entityName;

    public Repository(ApplicationContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _entityName = typeof(T).Name;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        try
        {
            return await _dbSet.FindAsync(id);
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                $"{_entityName} ID'sine göre sorgulanırken hata oluştu.",
                ex
            );
        }
    }

    public async Task<List<T>> GetAllAsync()
    {
        try
        {
            return await _dbSet.ToListAsync();
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                $"{_entityName} listesi alınırken hata oluştu.",
                ex
            );
        }
    }

    public async Task<T> AddAsync(T entity)
    {
        if (entity == null)
        {
            throw new ValidationException($"{_entityName} nesnesi null olamaz.");
        }

        try
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                $"{_entityName} eklenirken hata oluştu.",
                ex
            );
        }
    }

    public async Task<T> UpdateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ValidationException($"{_entityName} nesnesi null olamaz.");
        }

        try
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                $"{_entityName} güncellenirken hata oluştu.",
                ex
            );
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(
                    $"{_entityName} (ID: {id}) bulunamadı."
                );
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                $"{_entityName} silinirken hata oluştu.",
                ex
            );
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await _dbSet.FindAsync(id) != null;
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                $"{_entityName} varlık kontrolü yapılırken hata oluştu.",
                ex
            );
        }
    }
}
