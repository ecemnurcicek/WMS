using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Context;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class ApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    #region DbSets

    // Identity DbSets (inherited from IdentityDbContext)
    // Users, Roles, UserRoles, etc.

    // Business DbSets
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Region> Regions { get; set; } = null!;
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<Town> Towns { get; set; } = null!;
    public DbSet<Shop> Shops { get; set; } = null!;
    public DbSet<Warehouse> Warehouses { get; set; } = null!;
    public DbSet<Shelf> Shelves { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductShelf> ProductShelves { get; set; } = null!;
    public DbSet<ProductTransaction> ProductTransactions { get; set; } = null!;
    public DbSet<Transfer> Transfers { get; set; } = null!;
    public DbSet<TransferDetail> TransferDetails { get; set; } = null!;
    public DbSet<Log> Logs { get; set; } = null!;

    // Custom User Management DbSets
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;

    // Menu Management DbSets
    public DbSet<Menu> Menus { get; set; } = null!;
    public DbSet<MenuRole> MenuRoles { get; set; } = null!;

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure table names to match SQL script
        modelBuilder.Entity<Brand>().ToTable("Brands");
        modelBuilder.Entity<Region>().ToTable("Regions");
        modelBuilder.Entity<City>().ToTable("Cities");
        modelBuilder.Entity<Town>().ToTable("Towns");
        modelBuilder.Entity<Shop>().ToTable("Shops");
        modelBuilder.Entity<Warehouse>().ToTable("Warehouses");
        modelBuilder.Entity<Shelf>().ToTable("Shelves");
        modelBuilder.Entity<Product>().ToTable("Products");
        modelBuilder.Entity<ProductShelf>().ToTable("ProductShelves");
        modelBuilder.Entity<ProductTransaction>().ToTable("ProductTransactions");
        modelBuilder.Entity<Transfer>().ToTable("Transfers");
        modelBuilder.Entity<TransferDetail>().ToTable("TransferDetails");
        modelBuilder.Entity<Log>().ToTable("Logs");
        modelBuilder.Entity<Menu>().ToTable("Menus");
        modelBuilder.Entity<MenuRole>().ToTable("MenuRoles");

        // Configure Identity tables
        modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
        modelBuilder.Entity<ApplicationRole>().ToTable("AspNetRoles");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens");

        // Brand configuration
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // Region configuration
        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RegionName).HasColumnName("RegionName").HasMaxLength(100).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // City configuration
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CityName).HasColumnName("CityName").HasMaxLength(100).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasOne(e => e.Region).WithMany(r => r.Cities).HasForeignKey(e => e.RegionId);
        });

        // Town configuration
        modelBuilder.Entity<Town>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasOne(e => e.City).WithMany(c => c.Towns).HasForeignKey(e => e.CityId);
        });

        // Shop configuration
        modelBuilder.Entity<Shop>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasOne(e => e.Brand).WithMany(b => b.Shops).HasForeignKey(e => e.BrandId);
            entity.HasOne(e => e.Town).WithMany(t => t.Shops).HasForeignKey(e => e.TownId);
        });

        // Warehouse configuration
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasOne(e => e.Shop).WithMany(s => s.Warehouses).HasForeignKey(e => e.ShopId);
        });

        // Shelf configuration
        modelBuilder.Entity<Shelf>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasOne(e => e.Warehouse).WithMany(w => w.Shelves).HasForeignKey(e => e.WarehouseId);
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Model).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Size).HasMaxLength(20);
            entity.Property(e => e.CoverUrl).HasColumnName("CoverUrl").HasColumnType("TEXT");
            entity.Property(e => e.Description).HasColumnName("Description").HasColumnType("TEXT");
            entity.Property(e => e.Price).HasColumnType("REAL");
            entity.Property(e => e.Ean).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnType("DATETIME").HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.UpdatedAt).HasColumnType("DATETIME");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // ProductShelf configuration
        modelBuilder.Entity<ProductShelf>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasColumnName("Quantity");
            entity.HasOne(e => e.Shelf).WithMany(s => s.ProductShelves).HasForeignKey(e => e.ShelfId);
            entity.HasOne(e => e.Product).WithMany(p => p.ProductShelves).HasForeignKey(e => e.ProductId);
        });

        // ProductTransaction configuration
        modelBuilder.Entity<ProductTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionType).HasColumnName("TransactionType").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Quantity).HasColumnName("Quantity");
            entity.Property(e => e.CreatedAt).HasColumnType("DATETIME").HasDefaultValueSql("datetime('now')");
            entity.HasOne(e => e.Product).WithMany(p => p.Transactions).HasForeignKey(e => e.ProductId);
            entity.HasOne(e => e.FromShelf).WithMany(s => s.FromTransactions).HasForeignKey(e => e.FromShelfId);
            entity.HasOne(e => e.ToShelf).WithMany(s => s.ToTransactions).HasForeignKey(e => e.ToShelfId);
            entity.HasOne(e => e.Transfer).WithMany(t => t.Transactions).HasForeignKey(e => e.TransferId);
        });

        // Transfer configuration
        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.CreatedAt).HasColumnType("DATETIME").HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.UpdatedAt).HasColumnType("DATETIME");
            entity.Property(e => e.Status).HasDefaultValue(0);
            entity.HasOne(e => e.FromShop).WithMany(s => s.FromTransfers).HasForeignKey(e => e.FromShopId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.ToShop).WithMany(s => s.ToTransfers).HasForeignKey(e => e.ToShopId).OnDelete(DeleteBehavior.NoAction);
        });

        // TransferDetail configuration
        modelBuilder.Entity<TransferDetail>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuantityRequired).HasColumnName("QuantityRequired");
            entity.Property(e => e.QuantitySent).HasColumnName("QuantitySent");
            entity.Property(e => e.CreatedAt).HasColumnType("DATETIME").HasDefaultValueSql("datetime('now')");
            entity.HasOne(e => e.Transfer).WithMany(t => t.Details).HasForeignKey(e => e.TransferId);
            entity.HasOne(e => e.Product).WithMany(p => p.TransferDetails).HasForeignKey(e => e.ProductId);
        });

        // Log configuration
        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ControllerName).HasColumnName("ControllerName").HasMaxLength(100);
            entity.Property(e => e.LineNumber).HasColumnName("LineNumber");
            entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("DATETIME").HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.CreatedBy).HasColumnName("CreatedBy");
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Users");
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Password).HasMaxLength(250).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("DATETIME").HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.CreatedBy).HasColumnName("CreatedBy");
            entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("DATETIME");
            entity.Property(e => e.UpdatedBy).HasColumnName("UpdatedBy");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.BirthDate).HasColumnName("BirthDate");
            entity.Property(e => e.ShopId).HasColumnName("ShopId");
            entity.HasOne(e => e.Shop).WithMany(s => s.Users).HasForeignKey(e => e.ShopId).IsRequired(false);

        });


        // Role configuration
        modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Roles");
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("DATETIME").HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.CreatedBy).HasColumnName("CreatedBy");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("DATETIME");
                entity.Property(e => e.UpdatedBy).HasColumnName("UpdatedBy");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

        // UserRole configuration
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("UserRoles");
            entity.Property(e => e.UserId).HasColumnName("UserId").IsRequired();
            entity.Property(e => e.RoleId).HasColumnName("RoleId").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("DATETIME").HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.CreatedBy).HasColumnName("CreatedBy");
            entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("DATETIME");
            entity.Property(e => e.UpdatedBy).HasColumnName("UpdatedBy");
            entity.HasOne(e => e.User).WithMany(u => u.UserRoles).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Role).WithMany(r => r.UserRoles).HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Cascade);
        });

        // ApplicationUser configuration
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.CreatedAt).HasColumnType("DATETIME").HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.UpdatedAt).HasColumnType("DATETIME");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // ApplicationRole configuration
        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("DATETIME").HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.UpdatedAt).HasColumnType("DATETIME");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // Menu configuration
        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Path).HasMaxLength(250).IsRequired();
            entity.Property(e => e.IconName).HasMaxLength(100);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnType("DATETIME").HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.UpdatedAt).HasColumnType("DATETIME");
        });

        // MenuRole configuration
        modelBuilder.Entity<MenuRole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MenuId).IsRequired();
            entity.Property(e => e.RoleId).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("DATETIME").HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.UpdatedAt).HasColumnType("DATETIME");
            entity.HasOne(e => e.Menu).WithMany(m => m.MenuRoles).HasForeignKey(e => e.MenuId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Role).WithMany().HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
