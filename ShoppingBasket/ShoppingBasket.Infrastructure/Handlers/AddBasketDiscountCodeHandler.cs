using MediatR;
using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Core.Commands;
using ShoppingBasket.Core.Dtos;
using ShoppingBasket.Infrastructure.Persistence;

namespace ShoppingBasket.Infrastructure.Handlers;

public class AddBasketDiscountCodeHandler(AppDbContext db)
    : IRequestHandler<AddBasketDiscountCodeCommand, DiscountCodeResultDto>
{
    public async Task<DiscountCodeResultDto> Handle(
        AddBasketDiscountCodeCommand request, 
        CancellationToken ct)
    {
        var basket = await db.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.UserId == request.UserId, ct);

        if (basket is null)
            throw new KeyNotFoundException($"Basket for user {request.UserId} not found.");

        var discountCode = await db.DiscountCodes
            .FirstOrDefaultAsync(d =>
                d.Code.ToUpper().Equals(request.DiscountCode, StringComparison.CurrentCultureIgnoreCase), ct);
        if(discountCode is null) 
            throw new KeyNotFoundException($"Discount code {request.DiscountCode} does not exist");
        
        //todo use in get logic
        // var saleItemsTotal = basket.GetSaleItems()
        //     .Sum(i => i.Quantity * i.SalePrice.Value);

        var nonSaleItemsTotal = basket.GetNonSaleItems()
            .Sum(i => i.Quantity * i.RegularPrice);
        
        var discountAmount = nonSaleItemsTotal * discountCode.DiscountPercentage;
        // var finalTotal = (nonSaleItemsTotal - discountAmount) + saleItemsTotal;

        basket.AddDiscount(request.DiscountCode, discountAmount);

        await db.SaveChangesAsync(ct);

        return new DiscountCodeResultDto(
            basket.Id,
            request.DiscountCode,
            discountAmount
        );
    }
}
