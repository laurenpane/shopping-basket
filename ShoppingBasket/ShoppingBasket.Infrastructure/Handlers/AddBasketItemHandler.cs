using MediatR;
using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Core.Commands;
using ShoppingBasket.Core.Entities;
using ShoppingBasket.Core.Entities.Application.Interfaces;
using ShoppingBasket.Core.Exceptions;
using ShoppingBasket.Infrastructure.Persistence;

namespace ShoppingBasket.Infrastructure.Handlers;

public class AddBasketItemHandler(AppDbContext db, IShippingService shippingService, IStockService stockService)
    : IRequestHandler<AddBasketItemCommand, Guid>
{
    public async Task<Guid> Handle(AddBasketItemCommand request, CancellationToken ct)
    {
        var basket = await db.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.UserId == request.UserId, ct);

        var item = await db.Items
            .FirstOrDefaultAsync(i => i.Id == request.ItemId, ct);

        if (item is null)
        {
            throw new KeyNotFoundException($"Item {request.ItemId} not found.");
        }
        
        if (!await shippingService.IsValidCountryAsync(request.ShippingCountryCode, ct))
            throw new InvalidShippingCountryException(request.ShippingCountryCode);
        
        await stockService.ReserveStockAsync(request.ItemId, request.Quantity, ct);
        
        if (basket is null)
        {
            basket = new Basket(request.UserId, request.ShippingCountryCode);
            db.Baskets.Add(basket);
        }
        
        basket.AddBasketItem(request.ItemId, request.Quantity, item.RegularPrice, item.SalePrice);
        
        await db.SaveChangesAsync(ct);
        return basket.Id;
    }
}
