using MediatR;
using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Core.Dtos;
using ShoppingBasket.Core.Queries;
using ShoppingBasket.Infrastructure.Persistence;

namespace ShoppingBasket.Infrastructure.Handlers;

public class GetBasketSummaryHandler(AppDbContext db) : IRequestHandler<GetBasketSummaryQuery, BasketSummaryDto>
{
    public async Task<BasketSummaryDto> Handle(GetBasketSummaryQuery query, CancellationToken ct)
    {
        var basket = await db.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.UserId == query.UserId, ct);

        if (basket is null)
            throw new KeyNotFoundException($"Basket for user {query.UserId} not found.");
        
        // Could have introduced DateTime added to sort by instead
        var basketItems = basket.Items
            .OrderBy(i => i.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(i => new BasketItemDto
            (i.Id, i.RegularPrice, i.SalePrice, i.Quantity))
            .ToList();
        
        var subtotalNonSale = basket.GetSubtotalNonSale();
        var subtotalSale = basket.GetSubtotalSale();
        
        var discountAmount = 0m;

        if (!string.IsNullOrEmpty(basket.DiscountCode))
        {
            var discount = await db.DiscountCodes
                .FirstOrDefaultAsync(d => d.Code == basket.DiscountCode, ct);

            if (discount is null)
                throw new KeyNotFoundException($"Discount code {basket.DiscountCode} not found.");

            discountAmount = subtotalNonSale * discount.DiscountPercentage;
        }

        var shippingCountry = await db.ShippingCountries
            .FirstOrDefaultAsync(c => c.Code == basket.ShippingCountryCode, ct);

        if (shippingCountry is null)
            throw new KeyNotFoundException($"Shipping country {basket.ShippingCountryCode} not found.");
        //  ShippingCountry check exists when basket first created but extra validation could have been introduced here

        var subtotalWithVat = (subtotalNonSale + subtotalSale) * 1.2m;
        // VAT hardcoded to 20% as unlikely to change, but best practice would be to store this separately
        
        var total = subtotalWithVat + shippingCountry.Price - discountAmount;

        return new BasketSummaryDto(
            BasketId: basket.Id,
            Subtotal: subtotalNonSale + subtotalSale,
            SubtotalWithVat: subtotalWithVat,
            ShippingCountryCode: shippingCountry.Code,
            ShippingCost: shippingCountry.Price,
            DiscountAmountApplied: discountAmount,
            DiscountCodeApplied: basket.DiscountCode ?? "",
            Total: total,
            PageNumber: query.PageNumber,
            PageSize: query.PageSize,
            Items: basketItems
        );
    }
}
