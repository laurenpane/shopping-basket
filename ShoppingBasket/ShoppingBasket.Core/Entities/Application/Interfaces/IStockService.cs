namespace ShoppingBasket.Core.Entities.Application.Interfaces;

public interface IStockService
{
    Task<Stock> ReserveStockAsync(Guid itemId, int quantity, CancellationToken ct);

    
}