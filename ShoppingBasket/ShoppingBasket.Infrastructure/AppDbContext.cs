using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Core.Entities;

namespace ShoppingBasket.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Basket> Baskets => Set<Basket>();
    public DbSet<BasketItem> BasketItems => Set<BasketItem>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Stock> Stock => Set<Stock>();
}