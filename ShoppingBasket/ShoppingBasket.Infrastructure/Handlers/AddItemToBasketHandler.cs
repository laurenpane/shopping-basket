using MediatR;
using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Core.Commands;
using ShoppingBasket.Core.Entities;
using ShoppingBasket.Core.Entities.Application.Interfaces;
using ShoppingBasket.Core.Exceptions;
using ShoppingBasket.Infrastructure.Persistence;

namespace ShoppingBasket.Infrastructure.Handlers;

public class AddItemToBasketHandler : IRequestHandler<AddItemToBasketCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IShippingService _shippingService;
    private readonly IStockService _stockService;

    public AddItemToBasketHandler(AppDbContext db, IShippingService shippingService, IStockService stockService)
    {
        _db = db;
        _shippingService = shippingService;
        _stockService = stockService;
    }

    public async Task<Guid> Handle(AddItemToBasketCommand request, CancellationToken ct)
    {
        var basket = await _db.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.UserId == request.UserId, ct);
        
        if (!await _shippingService.IsValidCountryAsync(request.ShippingCountryCode, ct))
            throw new InvalidShippingCountryException(request.ShippingCountryCode);
        
        await _stockService.ReserveStockAsync(request.ItemId, request.Quantity, ct);
        
        if (basket is null)
        {
            basket = new Basket(request.UserId, request.ShippingCountryCode);
            _db.Baskets.Add(basket);
        }
        
        basket.AddBasketItem(request.ItemId, request.Quantity);
        
        await _db.SaveChangesAsync(ct);
        return basket.Id;
    }
}
