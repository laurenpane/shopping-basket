using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Core.Entities;

namespace ShoppingBasket.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Basket> Baskets => Set<Basket>();
    public DbSet<BasketItem> BasketItems => Set<BasketItem>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Stock> Stock => Set<Stock>();
    public DbSet<ShippingCountry> ShippingCountries => Set<ShippingCountry>();
    public DbSet<DiscountCode> DiscountCodes => Set<DiscountCode>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShippingCountry>()
            .HasKey(c => c.Code);

        modelBuilder.Entity<Basket>()
            .HasMany(b => b.Items)
            .WithOne()
            .HasForeignKey(i => i.BasketId);

        base.OnModelCreating(modelBuilder);
    }
}