using MediatR;
using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Core.Commands;
using ShoppingBasket.Infrastructure.Persistence;

namespace ShoppingBasket.Infrastructure.Handlers;

public class RemoveBasketItemHandler(AppDbContext db)
    : IRequestHandler<RemoveBasketItemCommand>
{

    public async Task Handle(RemoveBasketItemCommand request, CancellationToken ct)
    {
        
        var basket = await db.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.UserId == request.UserId, ct);

        if (basket is null)
            throw new KeyNotFoundException($"Basket not found for user {request.UserId}");
        
        var basketItem = basket.Items.FirstOrDefault(basketItem => basketItem.Id == request.BasketItemId);

        if (basketItem is null)
            throw new KeyNotFoundException($"BasketItem not found with Id {request.BasketItemId}");

        db.BasketItems.Remove(basketItem);

        await db.SaveChangesAsync(ct);
    }
}
