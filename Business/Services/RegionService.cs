using Business.Interfaces;
using Core.Dtos;
using Core.Entities;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class RegionService : IRegionService
    {
        private readonly ApplicationContext _context;
        public RegionService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<RegionDto>> GetAllAsync(bool pActive)
        {
            var list = await _context.Regions
                                 .Select(r => new RegionDto
                                 {
                                     Id = r.Id,
                                     Name = r.RegionName,
                                     IsActive = r.IsActive
                                 })
                                 .ToListAsync();
            
            //Eğer kullanıcı rolünde ise sadece aktif olanları getirir.
            if (pActive)
                list = list.Where(r => r.IsActive).ToList();

            return list;
        }

        public async Task<RegionDto?> GetByIdAsync(int pId)
        {
            var region = await _context.Regions.FindAsync(pId);
            if (region == null) return null;

            var item = new RegionDto
            {
                Id = region.Id,
                Name = region.RegionName,
                IsActive = region.IsActive
            };
            return item;
        }

        public async Task<RegionDto> AddAsync(RegionDto pModel)
        {
            if (string.IsNullOrWhiteSpace(pModel.Name))
                throw new Exception("Bölge adı boş olamaz");

            var entity = new Region
            {
                RegionName = pModel.Name,
                IsActive = true
            };

            _context.Regions.Add(entity);
            await _context.SaveChangesAsync();

            pModel.Id = entity.Id;
            pModel.IsActive = true;
            return pModel;
        }

        public async Task<bool> UpdateAsync(RegionDto pModel)
        {
            var retVal = false;
            var region = await _context.Regions.FindAsync(pModel.Id);
            if (region == null)
                throw new Exception("Bölge bulunamadı");

            region.Id = pModel.Id;
            region.RegionName = pModel.Name;
            region.IsActive = pModel.IsActive;

            _context.Regions.Update(region);
            await _context.SaveChangesAsync();
            retVal = true;

            return retVal;
        }

        public async Task<bool> DeleteAsync(int pId)
        {
            var retVal = false;

            var region = await _context.Regions.FindAsync(pId);
            if (region == null)
                return retVal;

            region.IsActive = false;
            _context.Regions.Update(region);
            await _context.SaveChangesAsync();
            retVal = true;

            return retVal;
        }
    }
} 


