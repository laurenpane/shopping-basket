using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Core.Entities;
using ShoppingBasket.Core.Entities.Application.Interfaces;
using ShoppingBasket.Core.Exceptions;
using ShoppingBasket.Infrastructure.Persistence;

namespace ShoppingBasket.Infrastructure.Services;

public class StockService(AppDbContext db) : IStockService
{
    
    public async Task<Stock> ReserveStockAsync(Guid itemId, int quantity, CancellationToken ct)
    {
        var stock = await db.Stock
            .FirstOrDefaultAsync(s => s.ItemId == itemId, ct);

        if (stock is null || stock.QuantityAvailable < quantity)
            throw new InsufficientStockException(itemId);

        stock.QuantityAvailable -= quantity;
        return stock;
    }
    
    // Note: This is a simplified implementation that immediately decrements stock, and doesn't take into account other users' baskets.
    // In production, this may place a temporary hold on items (like retailers that hold items in your basket for 60 mins) rather than immediately reducing available quantity from stock.
}
