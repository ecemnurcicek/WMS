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
                Name = "Giriş Alanı",
                Path = "/EntryArea",
                IconName = "fas fa-warehouse",
                DisplayOrder = 11,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Menu
            {
                Name = "Kullanıcılar",
                Path = "/User",
                IconName = "fas fa-users",
                DisplayOrder = 12,
                IsActive = true,
                CreatedAt = DateTime.Now
            }
        };

        // Add menus to database
        await context.Menus.AddRangeAsync(menus);
        await context.SaveChangesAsync();

        // Assign menus to roles based on role-menu map
        var roleMenuMap = GetRoleMenuMap();
        var menuRoles = new List<MenuRole>();
        foreach (var menu in menus)
        {
            foreach (var role in roles)
            {
                bool shouldAssign;

                if (role.Name.Contains("Admin"))
                {
                    // Admin tüm menülere erişir
                    shouldAssign = true;
                }
                else if (roleMenuMap.TryGetValue(role.Name, out var allowedPaths))
                {
                    shouldAssign = allowedPaths.Contains(menu.Path);
                }
                else
                {
                    // Tanımlanmamış roller sadece Dashboard görür
                    shouldAssign = menu.Path == "/Dashboard";
                }

                if (shouldAssign)
                {
                    menuRoles.Add(new MenuRole
                    {
                        MenuId = menu.Id,
                        RoleId = role.Id,
                        CreatedAt = DateTime.Now
                    });
                }
            }
        }

        if (menuRoles.Any())
        {
            await context.MenuRoles.AddRangeAsync(menuRoles);
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Mevcut veritabanına "Kullanıcılar" menüsünü ekler (yoksa).
    /// Sadece Admin rolüne atanır.
    /// </summary>
    public static async Task EnsureUserMenuAsync(ApplicationContext context)
    {
        var exists = await context.Menus.AnyAsync(m => m.Path == "/User");
        if (exists) return;

        var adminRole = await context.Set<Role>().FirstOrDefaultAsync(r => r.Name.Contains("Admin"));
        if (adminRole == null) return;

        var menu = new Menu
        {
            Name = "Kullanıcılar",
            Path = "/User",
            IconName = "fas fa-users",
            DisplayOrder = 11,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        await context.Menus.AddAsync(menu);
        await context.SaveChangesAsync();

        var menuRole = new MenuRole
        {
            MenuId = menu.Id,
            RoleId = adminRole.Id,
            CreatedAt = DateTime.Now
        };

        await context.MenuRoles.AddAsync(menuRole);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Mevcut veritabanına "Giriş Alanı" menüsünü ekler (yoksa).
    /// Tüm rollere atanır.
    /// </summary>
    public static async Task EnsureEntryAreaMenuAsync(ApplicationContext context)
    {
        var exists = await context.Menus.AnyAsync(m => m.Path == "/EntryArea");
        if (exists) return;

        var menu = new Menu
        {
            Name = "Giriş Alanı",
            Path = "/EntryArea",
            IconName = "fas fa-warehouse",
            DisplayOrder = 11,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        await context.Menus.AddAsync(menu);
        await context.SaveChangesAsync();

        // Tüm rollere ata
        var allRoles = await context.Set<Role>().ToListAsync();
        var menuRoles = new List<MenuRole>();
        
        foreach (var role in allRoles)
        {
            menuRoles.Add(new MenuRole
            {
                MenuId = menu.Id,
                RoleId = role.Id,
                CreatedAt = DateTime.Now
            });
        }

        if (menuRoles.Any())
        {
            await context.MenuRoles.AddRangeAsync(menuRoles);
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Tüm roller için MenuRole kayıtlarını doğru şekilde senkronize eder.
    /// Her rolün sadece erişmesi gereken menülere erişimini sağlar.
    /// Fazla veya eksik kayıtları düzeltir.
    /// </summary>
    public static async Task EnsureMenuRolesForAllRolesAsync(ApplicationContext context)
    {
        var allRoles = await context.Set<Role>().ToListAsync();
        var allMenus = await context.Menus.ToListAsync();

        if (!allRoles.Any() || !allMenus.Any()) return;

        // Rol bazlı erişilebilir menü path'leri tanımla
        var roleMenuMap = GetRoleMenuMap();

        foreach (var role in allRoles)
        {
            // Bu rol için izin verilen menü path'lerini belirle
            List<string> allowedPaths;
            if (role.Name.Contains("Admin"))
            {
                // Admin tüm menülere erişir
                allowedPaths = allMenus.Select(m => m.Path).ToList();
            }
            else if (roleMenuMap.TryGetValue(role.Name, out var paths))
            {
                allowedPaths = paths;
            }
            else
            {
                // Tanımlanmamış roller sadece Dashboard görür
                allowedPaths = new List<string> { "/Dashboard" };
            }

            // Bu rolün mevcut MenuRole kayıtlarını al
            var existingMenuRoles = await context.MenuRoles
                .Where(mr => mr.RoleId == role.Id)
                .ToListAsync();

            // Fazla kayıtları sil (bu rolün erişmemesi gereken menüler)
            var toRemove = existingMenuRoles
                .Where(mr =>
                {
                    var menu = allMenus.FirstOrDefault(m => m.Id == mr.MenuId);
                    return menu != null && !allowedPaths.Contains(menu.Path);
                })
                .ToList();

            if (toRemove.Any())
            {
                context.MenuRoles.RemoveRange(toRemove);
            }

            // Eksik kayıtları ekle
            var existingMenuIds = existingMenuRoles.Select(mr => mr.MenuId).ToHashSet();
            var menusToAdd = allMenus
                .Where(m => allowedPaths.Contains(m.Path) && !existingMenuIds.Contains(m.Id))
                .ToList();

            foreach (var menu in menusToAdd)
            {
                context.MenuRoles.Add(new MenuRole
                {
                    MenuId = menu.Id,
                    RoleId = role.Id,
                    CreatedAt = DateTime.Now
                });
            }
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Her rol için erişilebilir menü path'lerini tanımlar.
    /// Admin rolü burada tanımlanmaz çünkü tüm menülere erişir.
    /// Yeni bir rol eklendiğinde buraya tanım eklenmeli.
    /// </summary>
    private static Dictionary<string, List<string>> GetRoleMenuMap()
    {
        return new Dictionary<string, List<string>>
        {
            ["Marka Sorumlusu"] = new List<string>
            {
                "/Dashboard",
                "/Brand",
                "/Product",
                "/Transfer",
                "/EntryArea"
            },
            ["Mağaza Sorumlusu"] = new List<string>
            {
                "/Dashboard",
                "/Shop",
                "/WareHouse",
                "/Shelf",
                "/Product",
                "/Transfer",
                "/EntryArea"
            },
            ["Satış Temsilcisi"] = new List<string>
            {
                "/Dashboard",
                "/Product",
                "/Transfer",
                "/EntryArea"
            }
        };
    }
}
