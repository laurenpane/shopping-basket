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
    
    private Task<bool> HasSufficientStockAsync(Guid itemId, int quantity, CancellationToken ct)
        => db.Stock.AnyAsync(stock => stock.ItemId == itemId && stock.QuantityAvailable >= quantity, ct);
}
