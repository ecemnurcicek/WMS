using Core.Entities;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Data;

public static class DatabaseSeeder
{
    public static async Task SeedMenusAsync(ApplicationContext context)
    {
        // Check if menus already exist
        if (await context.Menus.AnyAsync())
        {
            return;
        }

        // Get all roles for assignment
        var roles = await context.Set<Role>().ToListAsync();
        var adminRole = roles.FirstOrDefault(r => r.Name.Contains("Admin"));
        var allRoleIds = roles.Select(r => r.Id).ToList();

        // Define menus based on existing sidebar
        var menus = new List<Menu>
        {
            new Menu
            {
                Name = "Dashboard",
                Path = "/Dashboard",
                IconName = "fas fa-th-large",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Menu
            {
                Name = "Bölge Yönetimi",
                Path = "/Region",
                IconName = "fas fa-mountain",
                DisplayOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Menu
            {
                Name = "Şehir Yönetimi",
                Path = "/City",
                IconName = "fas fa-building",
                DisplayOrder = 3,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Menu
            {
                Name = "İlçe Yönetimi",
                Path = "/Town",
                IconName = "fas fa-map-marker-alt",
                DisplayOrder = 4,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Menu
            {
                Name = "Marka Yönetimi",
                Path = "/Brand",
                IconName = "fas fa-tags",
                DisplayOrder = 5,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Menu
            {
                Name = "Mağaza Yönetimi",
                Path = "/Shop",
                IconName = "fas fa-store",
                DisplayOrder = 6,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Menu
            {
                Name = "Depo Yönetimi",
                Path = "/WareHouse",
                IconName = "fas fa-warehouse",
                DisplayOrder = 7,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Menu
            {
                Name = "Raf Yönetimi",
                Path = "/Shelf",
                IconName = "fas fa-th",
                DisplayOrder = 8,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Menu
            {
                Name = "Ürünler",
                Path = "/Product",
                IconName = "fas fa-boxes",
                DisplayOrder = 9,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Menu
            {
                Name = "Transferler",
                Path = "/Transfer",
                IconName = "fas fa-exchange-alt",
                DisplayOrder = 10,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Menu
            {
                Name = "Raporlar",
                Path = "/Report",
                IconName = "fas fa-chart-bar",
                DisplayOrder = 11,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Menu
            {
                Name = "Sistem Ayarları",
                Path = "/Settings",
                IconName = "fas fa-cog",
                DisplayOrder = 12,
                IsActive = true,
                CreatedAt = DateTime.Now
            }
        };

        // Add menus to database
        await context.Menus.AddRangeAsync(menus);
        await context.SaveChangesAsync();

        // Assign all menus to all roles initially
        // You can customize this based on your requirements
        var menuRoles = new List<MenuRole>();
        foreach (var menu in menus)
        {
            foreach (var roleId in allRoleIds)
            {
                menuRoles.Add(new MenuRole
                {
                    MenuId = menu.Id,
                    RoleId = roleId,
                    CreatedAt = DateTime.Now
                });
            }
        }

        if (menuRoles.Any())
        {
            await context.MenuRoles.AddRangeAsync(menuRoles);
            await context.SaveChangesAsync();
        }
    }
}
