using Business.Interfaces;
using Core.Dtos;
using Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ShelfService : IShelfService
    {
        private readonly ApplicationContext _context;
        public ShelfService(ApplicationContext context)
        {
            _context = context;
        }

        public Task<ShelfDto> AddAsync(ShelfDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ShelfDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ShelfDto?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ShelfDto>> GetByWarehouseIdAsync(int warehouseId)
        {
            throw new NotImplementedException();
        }

        public Task<ShelfDto> UpdateAsync(ShelfDto dto)
        {
            throw new NotImplementedException();
        }
    }
}

    